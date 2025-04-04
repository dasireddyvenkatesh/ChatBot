using ChatBot;
using ChatBot.BusinessLayer.Classes;
using ChatBot.BusinessLayer.Interfaces;
using ChatBot.Repoistory.Classes;
using ChatBot.Repoistory.Interfaces;
using Microsoft.AspNetCore.Http.Connections;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSignalR(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromHours(4);
    options.HandshakeTimeout = TimeSpan.FromHours(1);
    options.KeepAliveInterval = TimeSpan.FromHours(2);
});

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    // Fetch MongoDB connection string from configuration
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    return new MongoClient(connectionString); 
});

builder.Services.AddTransient<ILastSeenStatus, LastSeenStatus>();
builder.Services.AddTransient<IGetUserChatHistory, GetUserChatHistory>();
builder.Services.AddTransient<IChatBotRepo, ChatBotRepo>();
builder.Services.AddTransient<IUserSaveHistory, UserSaveHistory>();
builder.Services.AddTransient<IGetChatDetails, GetChatDetails>();
builder.Services.AddTransient<ICompressImage, CompressImage>();
builder.Services.AddTransient<IInsertAndDuplicateCheckHistory, InsertAndDuplicateCheckHistory>();
builder.Services.AddTransient<INewUserRegistration, NewUserRegistration>();
builder.Services.AddTransient<IRegisterEmail, RegisterEmail>();
builder.Services.AddTransient<IVerifyEmailOtp, VerifyEmailOtp>();
builder.Services.AddTransient<IEmailMessage, EmailMessage>();
builder.Services.AddTransient<IRegisterVerifiedEmail, RegisterVerifiedEmail>();
builder.Services.AddTransient<IResendEmailOtp, ResendEmailOtp>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()       // Allows any origin
              .AllowAnyMethod()       // Allows any HTTP method (GET, POST, etc.)
              .AllowAnyHeader();      // Allows any header
    });
});


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseMiddleware<CustomMiddleware>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Public}/{action=ChatBotInital}/{id?}");
    endpoints.MapHub<ChatHub>("/ChatHub", options =>
    {
        options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
        options.TransportSendTimeout = TimeSpan.FromDays(1);
    });
});

app.UseCors("AllowAllOrigins");

app.Run();
