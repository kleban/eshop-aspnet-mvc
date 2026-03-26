# 02 — Управління товарами

## Що реалізуємо?

У цьому розділі додаємо повноцінну роботу з товарами:

- Сутності: `Product`, `UnitOfMeasure`, `ProductType`
- Репозиторій `IProductRepository` та розширення `IUnitOfWork`
- Seed статичних даних через `HasData` (одиниці виміру)
- Seed тестових товарів через `DbInitializer` (при старті)
- `ProductController` — публічний каталог + адмін-управління
- Views: каталог з картками Bootstrap, деталі, CRUD для адміністратора
- Захист дій через `[Authorize(Roles = "Admin,Manager")]`

---

## Архітектура рішення

```
eShop.Core
  Entities/ProductData/
    Product.cs          — основна сутність товару
    ProductType.cs      — enum: Countable / Serialized / Bulk
    UnitOfMeasure.cs    — одиниця виміру (шт, кг, л)
    ProductImage.cs     — зображення товару
    ProductItem.cs      — конкретний екземпляр (для серійних)
    Inventory.cs        — залишки на складі

eShop.Infrastructure
  Interfaces/
    IProductRepository.cs
    IUnitOfWork.cs        — додаємо Product та Unit
  Repositories/
    ProductRepository.cs
    UnitOfWork.cs         — реєструємо нові репозиторії
  Context/
    ShopDataSeedExtension.cs — seed UnitOfMeasure через HasData
    DbInitializer.cs         — seed тестових товарів при старті

eShop.App
  ViewModels/Product/
    ProductCardViewModel.cs    — для картки в каталозі
    ProductCatalogViewModel.cs — каталог: список + фільтри
    ProductViewModel.cs        — деталі / видалення
    CreateProductViewModel.cs  — форма створення
    EditProductViewModel.cs    — форма редагування
  Controllers/
    ProductController.cs
  Views/Product/
    Index.cshtml    — каталог (картки Bootstrap)
    Details.cshtml  — деталі товару
    Manage.cshtml   — адмін-таблиця
    Create.cshtml
    Edit.cshtml
    Delete.cshtml
```

---

## Крок 1 — Доменні сутності

### ProductType (enum)

```csharp
// eShop.Core/Entities/ProductData/ProductType.cs
public enum ProductType
{
    Countable,   // поштучно, без серійників
    Serialized,  // поштучно, серійні номери
    Bulk         // вага, об'єм
}
```

Також додаємо extension-метод для відображення у UI:

```csharp
public static class ProductTypeExtensions
{
    public static string ToDisplayName(this ProductType type) => type switch
    {
        ProductType.Countable  => "Поштучний",
        ProductType.Serialized => "Серійний",
        ProductType.Bulk       => "На вагу / об'єм",
        _                      => type.ToString()
    };
}
```

### UnitOfMeasure

```csharp
// eShop.Core/Entities/ProductData/UnitOfMeasure.cs
public class UnitOfMeasure
{
    public int Id { get; set; }
    public string Name { get; set; }   // Штука, Кілограм, Літр
    public string Symbol { get; set; } // шт, кг, л
}
```

### Product

```csharp
// eShop.Core/Entities/ProductData/Product.cs
public class Product : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }

    public ProductType Type { get; set; }

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public int? BaseUnitId { get; set; }
    public UnitOfMeasure? BaseUnit { get; set; }

    public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public virtual ICollection<ProductItem> Items { get; set; } = new List<ProductItem>();
}
```

### Типи товарів — в чому різниця?

| Тип | Опис | Залишок |
|---|---|---|
| `Countable` | Звичайний товар поштучно (зарядний кабель, чохол) | `Inventory.Quantity` |
| `Serialized` | Кожен екземпляр має серійний номер (ноутбук, телефон) | кількість `ProductItem` зі статусом `InStock` |
| `Bulk` | Продається на вагу чи об'єм (кава, олія) | `Inventory.Quantity` в одиницях виміру |

---

## Крок 2 — Precision у ShopContext

Для грошових полів і полів кількості налаштовуємо точність у `OnModelCreating`:

```csharp
// Гроші
modelBuilder.Entity<Product>()
    .Property(p => p.Price).HasPrecision(18, 2);

modelBuilder.Entity<Order>()
    .Property(o => o.TotalAmount).HasPrecision(18, 2);

// Кількість / вага
modelBuilder.Entity<Inventory>()
    .Property(i => i.Quantity).HasPrecision(18, 4);

modelBuilder.Entity<CartItem>()
    .Property(ci => ci.Quantity).HasPrecision(18, 4);
```

`decimal` у SQL Server без `HasPrecision` отримує тип `decimal(18,2)` за замовчуванням — але краще вказати явно, щоб міграція не генерувала зайвих попереджень.

---

## Крок 3 — IProductRepository

```csharp
// eShop.Infrastructure/Interfaces/IProductRepository.cs
public interface IProductRepository : IRepository<Product>
{
    void Update(Product entity);
}
```

Успадковуємо від `IRepository<Product>` — отримуємо `GetAll`, `Get`, `Add`, `Remove` безкоштовно. Метод `Update` додаємо окремо — він потрібен для часткового оновлення полів без перезапису навігаційних властивостей.

### ProductRepository

```csharp
// eShop.Infrastructure/Repositories/ProductRepository.cs
public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ShopContext ctx) : base(ctx) { }

    public void Update(Product entity)
    {
        var fromDb = _ctx.Products.FirstOrDefault(p => p.Id == entity.Id);
        if (fromDb is null) return;

        fromDb.Name        = entity.Name;
        fromDb.Description = entity.Description;
        fromDb.Price       = entity.Price;
        fromDb.Type        = entity.Type;
        fromDb.CategoryId  = entity.CategoryId;
        fromDb.BaseUnitId  = entity.BaseUnitId;
    }
}
```

Чому не `_ctx.Update(entity)`? Метод `_ctx.Update()` помічає **всю** сутність як змінену, включно з навігаційними властивостями. Ручне копіювання полів дає точний контроль — ми змінюємо тільки те, що прийшло з форми.

---

## Крок 4 — Розширення IUnitOfWork

Додаємо два нові репозиторії до інтерфейсу:

```csharp
// eShop.Infrastructure/Interfaces/IUnitOfWork.cs
public interface IUnitOfWork
{
    ICategoryRepository Category { get; }
    IAddressRepository Address { get; }
    IProductRepository Product { get; }
    IRepository<UnitOfMeasure> Unit { get; }  // для select-списків у формах
    void Save();
    Task SaveAsync();
}
```

`IRepository<UnitOfMeasure>` без окремого інтерфейсу — одиниці виміру не потребують специфічних методів, базових `GetAll`/`Get` достатньо.

### UnitOfWork

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly ShopContext _ctx;

    public ICategoryRepository Category { get; }
    public IAddressRepository Address { get; }
    public IProductRepository Product { get; }
    public IRepository<UnitOfMeasure> Unit { get; }

    public UnitOfWork(ShopContext ctx)
    {
        _ctx     = ctx;
        Category = new CategoryRepository(_ctx);
        Address  = new AddressRepository(_ctx);
        Product  = new ProductRepository(_ctx);
        Unit     = new Repository<UnitOfMeasure>(_ctx);
    }

    public void Save() => _ctx.SaveChanges();
    public async Task SaveAsync() => await _ctx.SaveChangesAsync();
}
```

### Program.cs

```csharp
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
```

---

## Крок 5 — Seed даних

### UnitOfMeasure через HasData

Одиниці виміру — статичні константи, ніколи не змінюються. Використовуємо `HasData` у `ShopDataSeedExtension.cs`:

```csharp
builder.Entity<UnitOfMeasure>().HasData(
    new UnitOfMeasure { Id = 1, Name = "Штука",    Symbol = "шт" },
    new UnitOfMeasure { Id = 2, Name = "Кілограм", Symbol = "кг" },
    new UnitOfMeasure { Id = 3, Name = "Літр",     Symbol = "л"  }
);
```

`HasData` вбудовує дані у міграцію — вони будуть у БД завжди, незалежно від стану застосунку.

### Тестові товари через DbInitializer

Товари — динамічні дані (адмін може змінювати). Тому сідінг — при старті:

```csharp
private static async Task SeedProductsAsync(ShopContext db)
{
    if (await db.Products.AnyAsync()) return; // ідемпотентність

    var products = new[]
    {
        new Product
        {
            Name        = "Ноутбук ASUS VivoBook 15",
            Description = "15.6\", Intel Core i5, 8 ГБ RAM, 512 ГБ SSD",
            Price       = 24999m,
            Type        = ProductType.Serialized,
            CategoryId  = 1,  // Ноутбуки
            BaseUnitId  = 1   // шт
        },
        // ... інші товари
    };

    db.Products.AddRange(products);
    await db.SaveChangesAsync();
}
```

**Ідемпотентність** — перевірка `AnyAsync()` гарантує, що при кожному перезапуску застосунку товари не дублюються.

Виклик у `SeedUsersAsync`:

```csharp
public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ShopContext>();

    await SeedProductsAsync(db);
    // ... seed users
}
```

---

## Крок 6 — ViewModels

### ProductCardViewModel — для каталогу

```csharp
public class ProductCardViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? CategoryName { get; set; }
    public string TypeDisplay { get; set; } = string.Empty;
}
```

### ProductCatalogViewModel — весь каталог

```csharp
public class ProductCatalogViewModel
{
    public IEnumerable<ProductCardViewModel> Products { get; set; } = [];
    public IEnumerable<Category> Categories { get; set; } = [];
    public int? SelectedCategoryId { get; set; }
    public string? Search { get; set; }
}
```

Передає в View і список товарів, і список категорій для фільтрації — в одному об'єкті.

### CreateProductViewModel / EditProductViewModel — форми

```csharp
public class CreateProductViewModel
{
    [Display(Name = "Назва")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Ціна (грн)")]
    public decimal Price { get; set; }

    [Display(Name = "Тип товару")]
    public ProductType Type { get; set; }

    [Display(Name = "Категорія")]
    public int? CategoryId { get; set; }

    [Display(Name = "Одиниця виміру")]
    public int? BaseUnitId { get; set; }

    // Select-списки для <select> у формі
    public IEnumerable<SelectListItem> CategoryList { get; set; } = [];
    public IEnumerable<SelectListItem> UnitList { get; set; } = [];
}
```

`SelectListItem` — стандартний ASP.NET Core клас для `<select>`. Список категорій і одиниць виміру потрібен як при GET (перший рендер), так і при невалідному POST (перерендер з помилками).

---

## Крок 7 — ProductController

### Структура контролера

```csharp
public class ProductController : BaseController
{
    public ProductController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

    // Публічні дії — доступні всім
    public IActionResult Index(int? categoryId, string? search) { ... }
    public IActionResult Details(int? id) { ... }

    // Адмін-дії — тільки Admin та Manager
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult Manage() { ... }

    [Authorize(Roles = "Admin,Manager")]
    public IActionResult Create() { ... }

    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int? id) { ... }
}
```

### Index — каталог з фільтрами

```csharp
public IActionResult Index(int? categoryId, string? search)
{
    IEnumerable<Product> products = _unitOfWork.Product
        .GetAll(includeProperties: "Category,BaseUnit");

    if (categoryId.HasValue)
        products = products.Where(p => p.CategoryId == categoryId.Value);

    if (!string.IsNullOrWhiteSpace(search))
        products = products.Where(p =>
            p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));

    var cards = products.Select(p => new ProductCardViewModel { ... }).ToList();

    var model = new ProductCatalogViewModel
    {
        Products           = cards,
        Categories         = _unitOfWork.Category.GetAll().OrderBy(c => c.DisplayOrder),
        SelectedCategoryId = categoryId,
        Search             = search
    };

    return View(model);
}
```

> **Важливо:** `GetAll()` повертає вже матеріалізований `List<T>` (виконує SQL-запит). Тому фільтрація (`Where`) відбувається в памʼяті на стороні застосунку (LINQ to Objects), а не в БД. Якщо товарів буде багато — варто перейти на `IQueryable` з фільтрацією до `ToList()`.

> **Обережно з `AsQueryable()`:** якщо викликати `.AsQueryable()` на вже матеріалізованому `IEnumerable<T>`, компілятор буде очікувати expression tree. В expression trees не підтримується null-propagating оператор `?.` — отримаємо помилку `CS8072`. Тому зберігаємо тип `IEnumerable<Product>`, а не `IQueryable<Product>`.

### Helper-методи для select-списків

```csharp
private CreateProductViewModel BuildCreateModel(CreateProductViewModel model)
{
    model.CategoryList = _unitOfWork.Category
        .GetAll()
        .OrderBy(c => c.DisplayOrder)
        .Select(c => new SelectListItem(c.Name, c.Id.ToString()));

    model.UnitList = _unitOfWork.Unit
        .GetAll()
        .Select(u => new SelectListItem(u.Name, u.Id.ToString()));

    return model;
}
```

Метод викликається і в GET (початковий рендер), і в POST при помилці валідації — щоб `<select>` знову мав список опцій.

---

## Крок 8 — Views

### Index.cshtml — каталог у вигляді карток

```html
<div class="row row-cols-1 row-cols-md-3 g-4">
    @foreach (var product in Model.Products)
    {
        <div class="col">
            <div class="card h-100 shadow-sm">
                <div class="card-body d-flex flex-column">
                    <span class="badge bg-secondary mb-2">@product.CategoryName</span>
                    <h5 class="card-title">@product.Name</h5>
                    <p class="card-text text-muted small flex-grow-1">@product.Description</p>
                    <span class="fs-5 fw-bold text-primary">@product.Price.ToString("N2") грн</span>
                </div>
                <div class="card-footer d-flex gap-2">
                    <a asp-action="Details" asp-route-id="@product.Id"
                       class="btn btn-outline-secondary btn-sm">Деталі</a>
                    <button type="button" class="btn btn-success btn-sm ms-auto" disabled>
                        🛒 Додати в кошик
                    </button>
                </div>
            </div>
        </div>
    }
</div>
```

`row-cols-md-3` — три колонки на середніх екранах, одна на мобільному. `h-100` на картці + `flex-grow-1` на description — вирівнює картки по висоті в рядку.

### Фільтрація по категоріях

```html
@foreach (var cat in Model.Categories)
{
    <a asp-action="Index"
       asp-route-categoryId="@cat.Id"
       asp-route-search="@Model.Search"
       class="btn btn-sm @(Model.SelectedCategoryId == cat.Id ? "btn-primary" : "btn-outline-secondary")">
        @cat.Name
    </a>
}
```

Фільтр категорій не скидає пошуковий рядок — `asp-route-search` передає поточне значення `Model.Search`.

### Create/Edit — enum у select

```html
@using eShop.Domain.Entities.ProductData

<select asp-for="Type" class="form-select"
        asp-items="Html.GetEnumSelectList<ProductType>()">
</select>
```

`Html.GetEnumSelectList<T>()` автоматично генерує `<option>` для кожного значення enum. Відображувані назви беруться з `[Display(Name = "...")]` атрибутів або — якщо їх немає — з імен значень enum.

---

## Крок 9 — Захист адмін-дій

```csharp
// Доступно Admin та Manager
[Authorize(Roles = "Admin,Manager")]
public IActionResult Manage() { ... }

// Видалення — тільки Admin
[Authorize(Roles = "Admin")]
public IActionResult Delete(int? id) { ... }
```

У View — умовний рендер кнопок залежно від ролі:

```html
@if (User.IsInRole("Admin") || User.IsInRole("Manager"))
{
    <a asp-action="Manage" class="btn btn-outline-primary btn-sm">Управління товарами</a>
}
```

---

## Крок 10 — Навігація

У `_Layout.cshtml` додаємо посилання на каталог для всіх і на управління товарами — тільки для адміністраторів:

```html
<li class="nav-item">
    <a class="nav-link" asp-controller="Product" asp-action="Index">Каталог</a>
</li>

@if (User.IsInRole("Admin") || User.IsInRole("Manager"))
{
    <li class="nav-item">
        <a class="nav-link" asp-controller="Product" asp-action="Manage">Товари</a>
    </li>
}
```

### Зміна default route

У `Program.cs` змінюємо маршрут за замовчуванням, щоб головна сторінка відкривала каталог:

```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}")
    .WithStaticAssets();
```

---

## Крок 11 — Міграція

Додаємо міграцію для seed UnitOfMeasure:

```bash
dotnet ef migrations add AddUnitOfMeasureSeed \
  --project eShop.Infrastructure \
  --startup-project eShop.App
```

Застосовуємо до БД:

```bash
dotnet ef database update \
  --project eShop.Infrastructure \
  --startup-project eShop.App
```

Міграція містить тільки `InsertData` для таблиці `UnitsOfMeasure` — жодних змін структури таблиць (вони вже є з попередньої міграції `AddDomainEntities`).

---

## Результат

Після запуску застосунку:

| URL | Доступ | Опис |
|---|---|---|
| `/` або `/Product` | Всі | Каталог товарів з картками |
| `/Product/Details/1` | Всі | Деталі товару |
| `/Product/Manage` | Admin, Manager | Таблиця всіх товарів |
| `/Product/Create` | Admin, Manager | Форма додавання |
| `/Product/Edit/1` | Admin, Manager | Форма редагування |
| `/Product/Delete/1` | Admin | Підтвердження видалення |

Seed-товари (додаються при першому запуску, якщо таблиця порожня):

| Назва | Категорія | Ціна |
|---|---|---|
| Ноутбук ASUS VivoBook 15 | Ноутбуки | 24 999 грн |
| Ноутбук Lenovo IdeaPad 3 | Ноутбуки | 19 499 грн |
| Планшет Samsung Galaxy Tab A9 | Планшети | 9 999 грн |
| Планшет Apple iPad 10 | Планшети | 16 999 грн |
| Смартфон Samsung Galaxy A55 | Телефони | 14 999 грн |
| Смартфон Xiaomi Redmi Note 13 | Телефони | 8 499 грн |
