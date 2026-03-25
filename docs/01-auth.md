# 01 — Автентифікація та авторизація з ASP.NET Core Identity

## Що таке ASP.NET Core Identity?

ASP.NET Core Identity — це вбудована система управління користувачами від Microsoft. Вона надає:

- Реєстрацію та вхід користувачів
- Хешування паролів (BCrypt)
- Ролі та claims
- Cookie-based автентифікацію
- Захист від CSRF

Identity не потрібно будувати з нуля — вона підключається кількома рядками і одразу дає готову інфраструктуру.

---

## Архітектурне рішення: де живе User?

У проєкті є три шари: `eShop.Core`, `eShop.Infrastructure`, `eShop.App`.

Клас `User : IdentityUser` розміщений у **eShop.Infrastructure**, а не в Core. Причина: `IdentityUser` — це EF-залежний клас (пов'язаний з таблицями БД). Core має залишатись чистим від інфраструктурних залежностей.

```
eShop.Core          — доменні сутності (Category, Product, Order...)
eShop.Infrastructure — User : IdentityUser, DbContext, репозиторії
eShop.App           — Controllers, Views, ViewModels
```

---

## Крок 1 — Додати NuGet пакет

Відкрий `eShop.Infrastructure/eShop.Infrastructure.csproj` та додай пакет:

```xml
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="10.0.5" />
```

Цей пакет надає `IdentityDbContext<TUser>` — розширену версію `DbContext` з підтримкою Identity таблиць.

> У `.App` проєкті окремий пакет не потрібен — `UserManager`, `SignInManager` та інші сервіси входять у стандартний ASP.NET Core Web SDK.

---

## Крок 2 — Клас User

Створи файл `eShop.Infrastructure/Entities/User.cs`:

```csharp
using Microsoft.AspNetCore.Identity;

namespace eShop.Infrastructure.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
```

### Що таке IdentityUser?

`IdentityUser` — базовий клас від Microsoft з вже готовими полями:
- `Id` (string GUID)
- `UserName`, `Email`, `NormalizedEmail`
- `PasswordHash`
- `SecurityStamp`, `ConcurrencyStamp`
- `PhoneNumber`, `EmailConfirmed`, тощо

Ми успадковуємо від нього і додаємо свої поля: `FirstName` та `LastName`.

---

## Крок 3 — Оновити DbContext

Відкрий `eShop.Infrastructure/Context/ShopContext.cs` і зміни базовий клас:

```csharp
// Було:
public class ShopContext : DbContext

// Стало:
public class ShopContext : IdentityDbContext<User>
```

`IdentityDbContext<User>` автоматично додає у міграцію всі Identity таблиці:
- `AspNetUsers` — користувачі
- `AspNetRoles` — ролі
- `AspNetUserRoles` — зв'язок користувач-роль
- `AspNetUserClaims`, `AspNetUserLogins`, `AspNetUserTokens`, `AspNetRoleClaims`

> Важливо викликати `base.OnModelCreating(modelBuilder)` у методі `OnModelCreating` — інакше Identity таблиці не налаштуються.

---

## Крок 4 — Seed ролей

Ролі (`Admin`, `Manager`, `Customer`) — це статичні дані. Для них підходить `HasData` у міграції.

У `ShopDataSeedExtension.cs` додай:

```csharp
builder.Entity<IdentityRole>().HasData(
    new IdentityRole { Id = "role-admin",    Name = "Admin",    NormalizedName = "ADMIN",    ConcurrencyStamp = "cs-role-admin" },
    new IdentityRole { Id = "role-manager",  Name = "Manager",  NormalizedName = "MANAGER",  ConcurrencyStamp = "cs-role-manager" },
    new IdentityRole { Id = "role-customer", Name = "Customer", NormalizedName = "CUSTOMER", ConcurrencyStamp = "cs-role-customer" }
);
```

### Чому треба хардкодити ConcurrencyStamp?

`IdentityRole` за замовчуванням генерує `ConcurrencyStamp = Guid.NewGuid()`. Це означає, що кожного разу коли EF перераховує модель — значення буде різним. EF Core побачить «зміну даних» і вимагатиме нову міграцію при кожному білді.

Рішення: задати фіксовані рядки для `ConcurrencyStamp`.

---

## Крок 5 — Seed користувачів через DbInitializer

Для користувачів `HasData` **не підходить** — `PasswordHasher` використовує випадкову сіль і генерує різний хеш при кожному виклику. Це ж призводить до нескінченних міграцій.

Правильний підхід — сідінг при старті застосунку через `UserManager`.

Створи `eShop.Infrastructure/Context/DbInitializer.cs`:

```csharp
public static class DbInitializer
{
    public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        await CreateUserAsync(userManager, "admin@eshop.com", "Admin123!", "Admin", "Admin", "Admin");
        await CreateUserAsync(userManager, "manager@eshop.com", "Manager123!", "Manager", "Manager", "Manager");
        await CreateUserAsync(userManager, "customer@eshop.com", "Customer123!", "Customer", "Customer", "Customer");
    }

    private static async Task CreateUserAsync(
        UserManager<User> userManager, string email, string password,
        string firstName, string lastName, string role)
    {
        if (await userManager.FindByEmailAsync(email) is not null)
            return; // вже існує — пропускаємо

        var user = new User { UserName = email, Email = email, EmailConfirmed = true,
                              FirstName = firstName, LastName = lastName };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(user, role);
    }
}
```

Метод ідемпотентний: якщо користувач вже є — нічого не відбувається.

---

## Крок 6 — Підключити Identity у Program.cs

```csharp
// 1. Реєструємо сервіси Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ShopContext>()  // зберігаємо в нашу БД
.AddDefaultTokenProviders();              // для скидання пароля тощо

// 2. Налаштовуємо cookie (куди редиректити)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});
```

У pipeline middleware **порядок важливий**:

```csharp
app.UseAuthentication(); // хто ти?
app.UseAuthorization();  // що тобі можна?
```

`UseAuthentication` має стояти **перед** `UseAuthorization`.

Запускаємо сід після `var app = builder.Build()`:

```csharp
await DbInitializer.SeedUsersAsync(app.Services);
```

---

## Крок 7 — ViewModels

### RegisterViewModel (`ViewModels/Account/RegisterViewModel.cs`)

```csharp
public class RegisterViewModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
```

### LoginViewModel (`ViewModels/Account/LoginViewModel.cs`)

```csharp
public class LoginViewModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}
```

ViewModel — це звичайний клас без логіки. Він описує **форму**, яку бачить користувач. Ніяких `[Required]` атрибутів — валідація через FluentValidation.

---

## Крок 8 — FluentValidation

Для кожного ViewModel створюємо валідатор у `Validators/Account/`.

### RegisterViewModelValidator

```csharp
public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
{
    public RegisterViewModelValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .Matches("[A-Z]").WithMessage("Пароль має містити хоча б одну велику літеру")
            .Matches("[0-9]").WithMessage("Пароль має містити хоча б одну цифру");
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .Equal(x => x.Password).WithMessage("Паролі не співпадають");
    }
}
```

Валідатори автоматично підхоплюються через `AddValidatorsFromAssemblyContaining<...>()` у `Program.cs` — нічого додатково реєструвати не треба.

---

## Крок 9 — AccountController

Контролер отримує `UserManager<User>` і `SignInManager<User>` через DI:

```csharp
public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
}
```

### Реєстрація (POST)

```csharp
var user = new User { UserName = model.Email, Email = model.Email, ... };
var result = await _userManager.CreateAsync(user, model.Password);

if (result.Succeeded)
{
    await _userManager.AddToRoleAsync(user, "Customer"); // роль за замовчуванням
    await _signInManager.SignInAsync(user, isPersistent: false);
    return RedirectToAction("Index", "Home");
}

foreach (var error in result.Errors)
    ModelState.AddModelError(string.Empty, error.Description);
```

### Вхід (POST)

```csharp
var result = await _signInManager.PasswordSignInAsync(
    model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

if (result.Succeeded)
    return Redirect(returnUrl ?? "/");
```

### Вихід (POST)

```csharp
await _signInManager.SignOutAsync();
return RedirectToAction("Index", "Home");
```

> Logout — завжди `[HttpPost]` з `[ValidateAntiForgeryToken]`. Якби це був GET, будь-який сайт міг би розлогінити користувача через `<img src="/Account/Logout">`.

---

## Крок 10 — Views

Форми у Razor використовують Tag Helpers з `asp-for`, `asp-action`, `asp-validation-for`:

```html
<form asp-action="Register" method="post">
    <div asp-validation-summary="ModelOnly" class="text-danger"></div>

    <div class="mb-3">
        <label asp-for="Email" class="form-label"></label>
        <input asp-for="Email" class="form-control" type="email" />
        <span asp-validation-for="Email" class="text-danger"></span>
    </div>

    <button type="submit" class="btn btn-primary">Зареєструватися</button>
</form>

@section Scripts {
    @{ await Html.RenderPartialAsync("_ValidationScriptsPartial"); }
}
```

`_ValidationScriptsPartial` підключає jQuery Validation — клієнтська валідація без перезавантаження сторінки.

---

## Крок 11 — Auth у навбарі (_Layout.cshtml)

```html
@if (User.Identity?.IsAuthenticated == true)
{
    <span class="nav-link">@User.Identity.Name</span>
    <form asp-controller="Account" asp-action="Logout" method="post">
        <button type="submit" class="btn btn-link nav-link">Вийти</button>
    </form>
}
else
{
    <a class="nav-link" asp-controller="Account" asp-action="Login">Вхід</a>
    <a class="nav-link" asp-controller="Account" asp-action="Register">Реєстрація</a>
}
```

`User` — це `ClaimsPrincipal`, доступний у будь-якому View через Razor. Після входу Identity автоматично заповнює його з cookie.

---

## Крок 12 — Авторизація з ролями ([Authorize])

```csharp
// Тільки для авторизованих
[Authorize]
public IActionResult Profile() { ... }

// Тільки для конкретних ролей
[Authorize(Roles = "Admin,Manager")]
public IActionResult Create() { ... }

// Дозволити всім (перекриває [Authorize] на класі)
[AllowAnonymous]
public IActionResult Index() { ... }
```

У CategoryController:
- `Index` — публічний (без атрибута)
- `Create`, `Edit` — тільки `Admin` та `Manager`
- `Delete` — тільки `Admin`

---

## Крок 13 — Міграція

Після всіх змін у коді — генеруємо міграцію:

```bash
dotnet ef migrations add AddIdentity \
  --project eShop.Infrastructure \
  --startup-project eShop.App
```

Застосовуємо до БД:

```bash
dotnet ef database update \
  --project eShop.Infrastructure \
  --startup-project eShop.App
```

Міграція створить Identity таблиці (`AspNetUsers`, `AspNetRoles`, `AspNetUserRoles` та ін.) та додасть у `AspNetRoles` три записи (Admin, Manager, Customer).

> Якщо `dotnet ef` недоступний — встанови глобально:
> ```bash
> dotnet tool install --global dotnet-ef
> ```

---

## Результат

Після запуску застосунку:

| URL | Поведінка |
|---|---|
| `/Account/Register` | Форма реєстрації — новий користувач отримує роль `Customer` |
| `/Account/Login` | Форма входу з `RememberMe` |
| `/Category/Create` | Редирект на Login, якщо не авторизовано |
| `/Category/Delete` | 403 AccessDenied для не-Admin |

Seed-користувачі (після запуску):

| Email | Пароль | Роль |
|---|---|---|
| admin@eshop.com | Admin123! | Admin |
| manager@eshop.com | Manager123! | Manager |
| customer@eshop.com | Customer123! | Customer |
