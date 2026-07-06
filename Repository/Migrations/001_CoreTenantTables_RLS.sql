-- =============================================================
-- Row-Level Security setup for core tenant-scoped tables.
-- Paste this inside the generated migration's Up() method via
-- migrationBuilder.Sql(@"..."), AFTER the CREATE TABLE statements
-- that EF Core generates for OrgModules, Teams, Roles, Users.
--
-- Strategy: the app connects as a restricted Postgres role
-- ("app_user") for all normal request traffic. Platform-admin
-- requests use a separate role ("app_platform_admin") that
-- bypasses RLS via BYPASSRLS, rather than threading an "is admin"
-- condition into every policy. This keeps policies simple and
-- keeps the bypass privilege visible at the Postgres role level,
-- which is easier to audit than a boolean buried in app code.
-- =============================================================

-- Roles (create once, outside of migrations that run per-deploy;
-- shown here for completeness — typically a one-time setup script)
-- CREATE ROLE app_user LOGIN PASSWORD '...';
-- CREATE ROLE app_platform_admin LOGIN PASSWORD '...' BYPASSRLS;

ALTER TABLE org_modules ENABLE ROW LEVEL SECURITY;
ALTER TABLE teams        ENABLE ROW LEVEL SECURITY;
ALTER TABLE roles        ENABLE ROW LEVEL SECURITY;
ALTER TABLE users        ENABLE ROW LEVEL SECURITY;

-- Force RLS even for the table owner (otherwise the owning role
-- bypasses RLS by default, which silently defeats the policy
-- during local dev when migrations run as the owner).
ALTER TABLE org_modules FORCE ROW LEVEL SECURITY;
ALTER TABLE teams        FORCE ROW LEVEL SECURITY;
ALTER TABLE roles        FORCE ROW LEVEL SECURITY;
ALTER TABLE users        FORCE ROW LEVEL SECURITY;

CREATE POLICY tenant_isolation_org_modules ON org_modules
    USING (organization_id = current_setting('app.current_org_id', true)::uuid);

CREATE POLICY tenant_isolation_teams ON teams
    USING (organization_id = current_setting('app.current_org_id', true)::uuid);

CREATE POLICY tenant_isolation_roles ON roles
    USING (organization_id = current_setting('app.current_org_id', true)::uuid);

CREATE POLICY tenant_isolation_users ON users
    USING (organization_id = current_setting('app.current_org_id', true)::uuid);

-- Note: current_setting(..., true) with the second arg = true returns
-- NULL instead of raising an error when the session variable hasn't
-- been set, which fails the comparison safely (no rows visible) rather
-- than throwing on, e.g., migration scripts or platform-admin sessions
-- that legitimately never call set_config.