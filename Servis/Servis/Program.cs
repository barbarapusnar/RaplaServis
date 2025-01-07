using Microsoft.EntityFrameworkCore;
using Servis.vaja;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<VajaContext>(opt => opt.UseMySQL());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "API";
    config.Title = "RaplaAPI v1";
    config.Version = "v1";
});
var app = builder.Build();

//app.MapGet("/", () => "Hello World!");
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "TodoAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

app.MapGet("/allocation", async (VajaContext db) =>
    await db.Categories.ToListAsync());
app.MapGet("/urnik",async (string predmet,VajaContext context) =>

await  
    (from al in context.Allocations
    join a in context.Appointments on al.AppointmentId equals a.Id
    join e in context.Events on a.EventId equals e.Id
    join ev in context.EventAttributeValues on e.Id equals ev.EventId
    join r in context.ResourceAttributeValues on al.ResourceId equals r.ResourceId
    join rr in context.RaplaResources on r.ResourceId equals rr.Id
    where a.AppointmentStart >= new DateTime(2024, 10, 1) && a.AppointmentStart <= new DateTime(2024, 10, 4)
          && ev.AttributeValue.Contains(predmet)
    group new { a, ev, r, rr } by new { a.AppointmentStart, a.AppointmentEnd, ev.AttributeKey, ev.AttributeValue } into grouped
    orderby grouped.Key.AppointmentStart.DayOfWeek, grouped.Key.AppointmentStart
    select new 
    {
        dan = grouped.Key.AppointmentStart.DayOfWeek.ToString(),
        zac = grouped.Key.AppointmentStart.ToString("HH:mm"),
        kon = grouped.Key.AppointmentEnd.ToString("HH:mm"),
        kljuc = grouped.Key.AttributeKey,
        vsebina = grouped.Key.AttributeValue,
        oseba = grouped.Where(x => x.rr.TypeKey == "person1").Max(x => x.r.AttributeValue),
        skupina = grouped.Where(x => x.rr.TypeKey == "resource7").Max(x => x.r.AttributeValue),
        prostor = grouped.Where(x => x.rr.TypeKey == "resource1").Max(x => x.r.AttributeValue)
    }).ToListAsync()
);
    
app.Run();
