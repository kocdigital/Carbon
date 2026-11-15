# Carbon.Audit transport packages

`Carbon.Audit` collects audit events (EF Core changes through `AuditInterceptor`, HTTP requests
through `RequestContextMiddleware`) and hands them to an `IAuditEventPublisher`. Since
Carbon.Audit 3.0.0 it carries no message transport, so it — and Carbon.WebApplication 6.0.0, which
depends on it — restores alongside either MassTransit major.

The RabbitMQ publisher lives in a transport package. Pick the one that matches the MassTransit
version the service already runs on, and reference only one:

| Service runs on | Package | Built against |
|---|---|---|
| MassTransit 8 | `Carbon.Audit.MassTransit` | Carbon.MassTransit 4.x, MassTransit 8.5.8 |
| MassTransit 7 | `Carbon.Audit.MassTransit7` | Carbon.MassTransit 3.11.0, MassTransit 7.3.1 |

Both packages ship the same assembly name, namespaces and API, so moving a service from
MassTransit 7 to 8 later is a package swap with no code change.

## When you need one

Only when `CarbonAudit:Enabled` is `true`. With auditing disabled, nothing is needed and nothing
connects to RabbitMQ.

```csharp
// Startup.ConfigureServices (Carbon.WebApplication already calls AddCarbonAudit)
services.AddCarbonAuditMassTransit(Configuration);
```

`AddCarbonAuditMassTransit` registers a dedicated `IAuditBus` from the `MassTransit:RabbitMq`
section and the `IAuditEventPublisher` that publishes to the `audit.events` and
`audit.httprequests` fanout exchanges. It does nothing when auditing is disabled and is safe to
call more than once.

## Upgrading from Carbon.WebApplication 4.6.20 – 5.2.0

Those versions registered the audit transport automatically. From 6.0.0, a service with
`CarbonAudit:Enabled: true` and no transport package stops at startup with an
`InvalidOperationException` naming the package to add — it does not run and silently drop audit
events. Services with auditing disabled need no change.

Carbon.WebApplication.Grpc 4.0.0 and Carbon.WebApplication.EntityFrameworkCore 3.0.0 move to the
same Carbon.Audit 3.0.0 and follow the same rule.

If the service registers its own `IAuditEventPublisher`, `AddCarbonAuditMassTransit` leaves it in
place and does not start the audit bus.

Upgrading can lower the resolved Carbon.MassTransit version: a MassTransit 7 service that only got
Carbon.MassTransit 3.11.0 through Carbon.Audit 1.5.0 falls back to the version its other packages
ask for. Reference Carbon.MassTransit 3.11.0 directly to keep it (it carries the net10.0 build).
