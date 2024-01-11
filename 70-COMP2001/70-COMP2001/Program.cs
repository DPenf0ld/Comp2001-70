

var builder = WebApplication.CreateBuilder(args); //builds web application

// Add services to the container.


//adds dependencies
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); //this is swagger
builder.Services.AddSwaggerGen();

var app = builder.Build(); //builds actual application

// Configure the HTTP request pipeline.]

//get rid of if statement when connecting to actual database
if (app.Environment.IsDevelopment()) //if environment isnt declared (in launchsettings) then it will pressume production 
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); //http sends to https

app.UseAuthorization();

app.MapControllers();

app.Run();
