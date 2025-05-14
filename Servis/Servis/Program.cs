using System.Security.Cryptography.Xml;
using Microsoft.EntityFrameworkCore;
using Servis.vaja;
using ZstdSharp.Unsafe;
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

app.MapGet("/urnikD",async (string predmet,string odD, string doD,VajaContext context) =>
{
    DateTime d1=DateTime.Parse(odD);
    DateTime d2=DateTime.Parse(doD);
    var rezultat=  
    await (from al in context.Allocations
    join a in context.Appointments on al.AppointmentId equals a.Id
    join e in context.Events on a.EventId equals e.Id
    join ev in context.EventAttributeValues on e.Id equals ev.EventId
    join r in context.ResourceAttributeValues on al.ResourceId equals r.ResourceId
    join rr in context.RaplaResources on r.ResourceId equals rr.Id
    where a.AppointmentStart >= d1 && a.AppointmentStart <= d2
          && ev.AttributeValue.Contains(predmet)
    group new { a, ev, r, rr } by new { a.AppointmentStart, a.AppointmentEnd, ev.AttributeValue } into grouped
    orderby grouped.Key.AppointmentStart.DayOfWeek, grouped.Key.AppointmentStart
    select new 
    {
        dan = grouped.Key.AppointmentStart.DayOfWeek.ToString(),
        zac = grouped.Key.AppointmentStart.ToString("HH:mm"),
        kon = grouped.Key.AppointmentEnd.ToString("HH:mm"),
        vsebina = grouped.Key.AttributeValue,
        oseba = grouped.Where(x => x.rr.TypeKey == "person1").Max(x => x.r.AttributeValue),
        
        skupina = grouped.Where(x => x.rr.TypeKey == "resource7").Max(x => x.r.AttributeValue),
        prostor = grouped.Where(x => x.rr.TypeKey == "resource1").Max(x => x.r.AttributeValue)
    }).ToListAsync();
    return Results.Json(rezultat);
    }
);
    app.MapGet("/urnik",async (string predmet,VajaContext context) =>

await  
    (from al in context.Allocations
    join a in context.Appointments on al.AppointmentId equals a.Id
    join e in context.Events on a.EventId equals e.Id
    join ev in context.EventAttributeValues on e.Id equals ev.EventId
    join r in context.ResourceAttributeValues on al.ResourceId equals r.ResourceId
    join rr in context.RaplaResources on r.ResourceId equals rr.Id
    where a.AppointmentStart >= new DateTime(2024, 5, 4) && a.AppointmentStart <= new DateTime(2024, 5, 12)
          && ev.AttributeValue.Contains(predmet)
    group new { a, ev, r, rr } by new { a.AppointmentStart, a.AppointmentEnd,  ev.AttributeValue } into grouped
    orderby grouped.Key.AppointmentStart.DayOfWeek, grouped.Key.AppointmentStart
    select new 
    {
        dan = grouped.Key.AppointmentStart.DayOfWeek.ToString(),
        zac = grouped.Key.AppointmentStart.ToString("HH:mm"),
        kon = grouped.Key.AppointmentEnd.ToString("HH:mm"),
        vsebina = grouped.Key.AttributeValue,
        oseba = grouped.Where(x => x.rr.TypeKey == "person1").Max(x => x.r.AttributeValue),
        skupina = grouped.Where(x => x.rr.TypeKey == "resource7").Max(x => x.r.AttributeValue),
        prostor = grouped.Where(x => x.rr.TypeKey == "resource1").Max(x => x.r.AttributeValue)
    }).ToListAsync()
);

app.MapGet("/predmeti", async(VajaContext context) =>
{
   var p=await( context.EventAttributeValues
        .Where(x => x.AttributeKey == "a1")
        .Select(x => x.AttributeValue)
        .Distinct()
        .OrderBy(x => x)
        .ToListAsync()
   );
    return Results.Json(p);
}
);

app.MapGet("/urnikRedni",async (string predmet,string odD, string doD,string s,string smer,VajaContext context) =>
{
    DateTime d1=DateTime.Parse(odD);
    int l1=d1.Year;int m1=d1.Month;int day1=d1.Day;
    DateTime d2=DateTime.Parse(doD);
    DateTime d3; //zaćetni datum, ko iščeš ponavljanja za dodajanje
    if (m1>=1 && m1<4)
    d3=new DateTime(l1, 1, 1);
    else if (m1>=4 && m1<7)
    d3=new DateTime(l1, 4, 1);
    else if (m1>=7 && m1<10)
    d3=new DateTime(l1, 7, 1);
    else if (m1>=10&&m1<=12)
    d3=new DateTime(l1, 10, 1);
    else d3=new DateTime(l1+1, 1, 1);
    var vse=DodajPonavljanja(d3,predmet,context); // dodaj ponavljanja za predmet
    //dobiti je treba predhodni datum, ima repetion in pade do tega datuma
    var rawData = (
    from x in vse

    join e in context.Events on x.EventId equals e.Id
    join ev in context.EventAttributeValues on e.Id equals ev.EventId
    join r in context.ResourceAttributeValues on x.ResourceId equals r.ResourceId
    join rr in context.RaplaResources on r.ResourceId equals rr.Id
    where x.AppointmentStart >= d1
       && x.AppointmentStart.Date <= d2
       && ev.AttributeValue.Contains(predmet) // ali ev.AttributeValue.Contains("") če res rabiš
    select new
    {
        ev.EventId,
        x.AppointmentStart,
        x.AppointmentEnd,
        ev.AttributeKey,
        ev.AttributeValue,  
       // ra=r.AttributeValue,
        ak=r.AttributeKey ,
        rr.TypeKey,
        atva=r.AttributeValue 
    }
    ).AsQueryable()
.AsEnumerable(); // ključni korak – preklop na klienta

// Grupiranje in logika na klientu
var podatki = rawData
    .GroupBy(x => new
    {
        Datum = x.AppointmentStart.Date,
        Dan = x.AppointmentStart.DayOfWeek.ToString(),
        Zac = x.AppointmentStart.TimeOfDay,
        Kon = x.AppointmentEnd.TimeOfDay,
        x.AttributeKey,
        x.AttributeValue,
        x.EventId
    })
    .Select(g =>
    {
        var oseba = g.Where(x => x.TypeKey == "person1"&&x.ak=="Ime").Select(x => x.atva).FirstOrDefault();
        var skupina = string.Join(",", g.Where(x => x.TypeKey == "resource7").Select(x => x.atva));
        var prostor = g.Where(x => x.TypeKey == "resource1").Select(x => x.atva).FirstOrDefault();

        return new
        {
            g.Key.Datum,
            g.Key.Dan,
            g.Key.Zac,
            g.Key.Kon,
            Ključ = g.Key.AttributeKey,
            Vsebina = g.Key.AttributeValue,
            Oseba = oseba,
            Skupina = skupina,
            Prostor = prostor
        };
    })
    .Where(x => !string.IsNullOrEmpty(x.Skupina)&&x.Ključ!="a1"&&x.Skupina.Contains(s) && !x.Skupina.Contains("IZREDNI", StringComparison.CurrentCultureIgnoreCase) && x.Skupina.Contains(smer))
    .OrderBy(x => x.Datum).ThenBy(x => x.Zac)
    .ToList();
    return Results.Json(podatki.ToList());
    }
);
app.MapGet("/urnikIzredni",async (string predmet,string odD, string doD,string s,string smer,VajaContext context) =>
{
    DateTime d1=DateTime.Parse(odD);
    int l1=d1.Year;int m1=d1.Month;int day1=d1.Day;
    DateTime d2=DateTime.Parse(doD);
    DateTime d3; //zaćetni datum, ko iščeš ponavljanja za dodajanje
    if (m1>=10)
    d3=new DateTime(l1, 10, 1);
    else 
    d3=new DateTime(l1-1, 10, 1);
    
    var vse=DodajPonavljanja(d3,predmet,context); // dodaj ponavljanja za predmet
    //dobiti je treba predhodni datum, ima repetion in pade do tega datuma
    var rawData = (
    from x in vse

    join e in context.Events on x.EventId equals e.Id
    join ev in context.EventAttributeValues on e.Id equals ev.EventId
    join r in context.ResourceAttributeValues on x.ResourceId equals r.ResourceId
    join rr in context.RaplaResources on r.ResourceId equals rr.Id
    where x.AppointmentStart >= d1
       && x.AppointmentStart.Date <= d2
       && ev.AttributeValue.Contains(predmet) // ali ev.AttributeValue.Contains("") če res rabiš
    select new
    {
        ev.EventId,
        x.AppointmentStart,
        x.AppointmentEnd,
        ev.AttributeKey,
        ev.AttributeValue,  
       // ra=r.AttributeValue,
        ak=r.AttributeKey ,
        rr.TypeKey,
        atva=r.AttributeValue 
    }
    ).AsQueryable()
.AsEnumerable(); // ključni korak – preklop na klienta

// Grupiranje in logika na klientu
var podatki = rawData
    .GroupBy(x => new
    {
        Datum = x.AppointmentStart.Date,
        Dan = x.AppointmentStart.DayOfWeek.ToString(),
        Zac = x.AppointmentStart.TimeOfDay,
        Kon = x.AppointmentEnd.TimeOfDay,
        x.AttributeKey,
        x.AttributeValue,
        x.EventId
    })
    .Select(g =>
    {
        var oseba = g.Where(x => x.TypeKey == "person1"&&x.ak=="Ime").Select(x => x.atva).FirstOrDefault();
        var skupina = string.Join(",", g.Where(x => x.TypeKey == "resource7").Select(x => x.atva));
        var prostor = g.Where(x => x.TypeKey == "resource1").Select(x => x.atva).FirstOrDefault();

        return new
        {
            g.Key.Datum,
            g.Key.Dan,
            g.Key.Zac,
            g.Key.Kon,
            Ključ = g.Key.AttributeKey,
            Vsebina = g.Key.AttributeValue,
            Oseba = oseba,
            Skupina = skupina,
            Prostor = prostor
        };
    })
    .Where(x => !string.IsNullOrEmpty(x.Skupina)&&x.Ključ!="a1"&&x.Skupina.Contains(s) && x.Skupina.Contains("IZREDNI", StringComparison.CurrentCultureIgnoreCase) && x.Skupina.Contains(smer))
    .OrderBy(x => x.Datum).ThenBy(x => x.Zac)
    .ToList();
    return Results.Json(podatki.ToList());
    }
);
List<Dodano> DodajPonavljanja(DateTime d3, string predmet, VajaContext context)
{
    var rawData = (
    from al in context.Allocations
    join a in context.Appointments on al.AppointmentId equals a.Id
    join e in context.Events on a.EventId equals e.Id
    join ev in context.EventAttributeValues on e.Id equals ev.EventId
    where a.AppointmentStart.Date >= d3
       && ev.AttributeValue.Contains(predmet) // ali ev.AttributeValue.Contains("") če res rabiš
    select new
    {
        ev.EventId,
        a.Id,
        a.AppointmentStart,
        a.AppointmentEnd,
        a.RepetitionType,
        a.RepetitionNumber,
        a.RepetitionInterval,
        al.ResourceId,
        al.AppointmentId
    }
    ).AsQueryable()
.AsEnumerable(); 
List<Dodano> duplicates = new List<Dodano>();
//List<dynamic> duplicatesal = new List<dynamic>();

foreach(var x in rawData)
{
    if (x.RepetitionType == "weekly" && x.RepetitionNumber > 0)
    {//najprej dodaj osnovnega, nato vsa ponavljanja
        duplicates.Add(new Dodano()
        {   AppointmentId=x.AppointmentId,
            EventId=x.EventId,
            AppointmentStart = x.AppointmentStart,
            AppointmentEnd = x.AppointmentEnd,
            ResourceId = x.ResourceId
        });
        for (int i = 1; i < x.RepetitionNumber; i++) //prvi repetition ima vrednost=0
        {
            DateTime newStart = x.AppointmentStart.AddDays(i * 7);
            DateTime newEnd = x.AppointmentEnd.AddDays(i * 7);
          
            string novID=Guid.NewGuid().ToString();
            duplicates.Add(new Dodano()
            {   AppointmentId=novID,
                EventId=x.EventId,
                AppointmentStart = newStart,
                AppointmentEnd = newEnd,
                ResourceId = x.ResourceId
            });
        }
    }
    else
    {
        duplicates.Add(new Dodano()
        {   AppointmentId=x.AppointmentId,
            EventId=x.EventId,
            AppointmentStart = x.AppointmentStart,
            AppointmentEnd = x.AppointmentEnd,
            ResourceId = x.ResourceId
        });
        
    }
    
   
}
return duplicates;
}

app.Run();

