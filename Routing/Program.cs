using Routing.CustomConstraints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options => {
    options.ConstraintMap.Add("months", typeof(MonthsCustomConstraint));
});

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.Map("files/{filename}/{extension}", async context =>
    {
        string? fileName = Convert.ToString(context.Request.RouteValues["filename"]);
        string? extension = Convert.ToString(context.Request.RouteValues["extension"]);

        await context.Response.WriteAsync($"In files - {fileName} - {extension}");
    });

    endpoints.Map("employee/profile/{EmployeeName:length(4,7): alpha=alex}", async context =>
    {
        string? EmployeeName = Convert.ToString(context.Request.RouteValues["EmployeeName"]);

        await context.Response.WriteAsync($"In Employee profile - {EmployeeName}");

    });

    endpoints.Map("products/details/{id:int:range(1,1000)?}", async context =>
    {
        if (context.Request.RouteValues.ContainsKey("id"))
        {
            int id = Convert.ToInt32(context.Request.RouteValues["id"]);

            await context.Response.WriteAsync($"Product id = {id}");
        }
        else
        {
            await context.Response.WriteAsync("Id is not supplied");
        }
    });


    endpoints.Map("daily-digest-report/{reportdate:datetime}", async context =>
    {
        DateTime reportDate = Convert.ToDateTime(context.Request.RouteValues["reportdate"]);
        await context.Response.WriteAsync($"Daily-digest-report - {reportDate.ToShortDateString()}");
    });

    endpoints.Map("sales-report/{year:int:min(1900)}/{month:months}", async context =>
    {
        int year = Convert.ToInt32(context.Request.RouteValues["year"]);
        string? month = Convert.ToString(context.Request.RouteValues["month"]);

        if (month == "apr" || month == "jul" || month == "oct" || month == "jan")
        {
            await context.Response.WriteAsync($"sales-report - {year} - {month}");
        }
        else
        {
            await context.Response.WriteAsync($"{month} is not allowed for sales report");

        }
    });

    endpoints.Map("sales-report/2024/jan", async context =>
    {
        await context.Response.WriteAsync("Sales report exclusively for 2024 - jan");
    });

});

app.Run(async context =>
{
    await context.Response.WriteAsync($"No route matched at {context.Request.Path}");
});

app.Run();