using Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace Infrastructure.Persistence
{
    /// <summary>
    /// Runs `SET app.current_org_id = '...'` on every DB connection right after
    /// it opens, reading from the same ICurrentTenantService that feeds the EF
    /// Core global query filters. This is what makes RLS and the query filters
    /// two independent enforcement layers reading one source of truth, instead
    /// of two places that could drift out of sync.
    ///
    /// Registered via optionsBuilder.AddInterceptors(...) in Program.cs.
    /// </summary>
    public class TenantConnectionInterceptor(ICurrentTenantService tenant) : DbConnectionInterceptor
    {
        private readonly ICurrentTenantService _tenant = tenant;

        public override async Task ConnectionOpenedAsync(
            DbConnection connection,
            ConnectionEndEventData eventData,
            CancellationToken cancellationToken = default)
        {
            await SetTenantSessionVariable(connection, cancellationToken);
        }

        public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
        {
            SetTenantSessionVariable(connection, CancellationToken.None).GetAwaiter().GetResult();
        }

        private async Task SetTenantSessionVariable(DbConnection connection, CancellationToken ct)
        {
            // Platform admins bypass RLS entirely via a separate Postgres role/policy
            // (see migration), so we only set this for tenant-scoped sessions.
            if (_tenant.IsPlatformAdmin || _tenant.OrganizationId is null)
            {
                return;
            }

            using var cmd = connection.CreateCommand();
            // set_config(..., is_local = false) keeps the setting for the connection's
            // lifetime in the pool; with pooling this is reset per logical session by
            // Npgsql — verify behavior against your pooling config before relying on
            // this in a high-concurrency scenario, and reset explicitly on connection close if needed.
            cmd.CommandText = "SELECT set_config('app.current_org_id', @orgId, false)";
            var param = cmd.CreateParameter();
            param.ParameterName = "orgId";
            param.Value = _tenant.OrganizationId.Value.ToString();
            cmd.Parameters.Add(param);

            await cmd.ExecuteNonQueryAsync(ct);
        }
    }
}
