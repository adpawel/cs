DatabaseService.Connect();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Dodanie obsługi sesji
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(30);
    options.Cookie.HttpOnly = true;  // plik cookie jest niedostępny przez skrypt po stronie klienta
    options.Cookie.IsEssential = true;  // pliki cookie sesji będą zapisywane dzięki czemu sesje będzie mogła być śledzona podczas nawigacji lub przeładowania strony
});

// Dodanie HttpContextAccessor, jeżeli potrzebujesz dostępu do sesji w innych komponentach
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Ustawienie, że sesja jest dostępna wcześniej
app.UseSession();  // <- Ważne, aby było przed sprawdzeniem autoryzacji

app.UseRouting();

app.Use(async (ctx, next) =>
{
    var path = ctx.Request.Path.Value.ToLower();
    
    // Zdefiniuj listę zasobów, które wymagają autoryzacji
    var protectedPaths = new[] { "/products", "/login/logout", "/home/privacy" };

    Console.WriteLine($"Request path: {path}");
    // Sprawdź, czy ścieżka pasuje do którejś z chronionych
    if (protectedPaths.Any(path.StartsWith))
    {
        // Jeśli użytkownik nie jest zalogowany, przekieruj na stronę logowania
        if (!ctx.Session.Keys.Contains("login"))
        {
            ctx.Response.Redirect("/Login/GetForm");
            return;  // Przerwij dalsze przetwarzanie
        }
    }

    // Jeśli użytkownik jest zalogowany, przejdź do następnego middleware
    await next();
});

// Następnie konfiguracja autoryzacji
app.UseAuthorization();

app.UseStatusCodePagesWithReExecute("/Login/GetForm");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

