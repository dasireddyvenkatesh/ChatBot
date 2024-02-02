using ChatBot;
using ChatBot.BusinessLayer.Classes;
using ChatBot.BusinessLayer.Interfaces;
using ChatBot.Repoistory.Classes;
using ChatBot.Repoistory.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddTransient<ILastSeenStatus, LastSeenStatus>();
builder.Services.AddTransient<IGetUserChatHistory, GetUserChatHistory>();
builder.Services.AddTransient<IChatBotRepo, ChatBotRepo>();
builder.Services.AddTransient<IUserSaveHistory, UserSaveHistory>();
builder.Services.AddTransient<IGetChatDetails, GetChatDetails>();
builder.Services.AddTransient<ICompressImage, CompressImage>();
builder.Services.AddTransient<IInsertAndDuplicateCheckHistory, InsertAndDuplicateCheckHistory>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=ChatBot}/{action=ChatBotInital}/{id?}");
    endpoints.MapHub<ChatHub>("/ChatHub");
});

app.Run();
