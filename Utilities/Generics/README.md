# Generic Repository Pattern - NET 8.0

Bu proje, Entity Framework Core ile birlikte kullanılan **Generic Repository Pattern** implementasyonunu sağlar. Veri erişim katmanını soyutlayarak, SOLID prensiplerine uygun ve test edilebilir bir mimari sunar.

## 📋 İçindekiler

- [Özellikler](#özellikler)
- [Kurulum](#kurulum)
- [Yapı](#yapı)
- [Kullanım Örnekleri](#kullanım-örnekleri)
- [API Referansı](#api-referansı)
- [En İyi Uygulamalar](#en-iyi-uygulamalar)
- [Uyarılar](#uyarılar)

## ✨ Özellikler

- **Generic Tasarım**: Tüm entity türleri için tek bir repository implementasyonu
- **Async/Await Desteği**: Tüm işlemler asenkron olarak gerçekleştirilir
- **Lambda Expression Desteği**: Güçlü sorgu yazma imkanı
- **Eager Loading**: `Include` parametresi ile ilişkili veriler yüklenebilir
- **SOLID Prensiplerine Uygun**: Interface-based tasarım ve dependency injection desteği
- **NET 8.0 Uyumlu**: Modern C# özellikleri kullanıyor

## 🚀 Kurulum

### Gereksinimler

- .NET 8.0 veya üstü
- Entity Framework Core 8.0+
- C# 12.0+

### Adımlar

1. Bu sınıfları projenize ekleyin:
```
Utilities/
└── Generics/
    ├── IRepository.cs
    └── Repository.cs
```

2. Entity Framework Core'u yükleyin:
```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

3. Dependency Injection'ı yapılandırın:
```csharp
services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
```

## 🏗️ Yapı

### IRepository<T> Interface

Repository'nin tüm işlemlerini tanımlayan generic interface.

**Type Constraint**: `where T : class`

### Repository<T> Abstract Class

IRepository<T> interface'ini implement eden abstract sınıf. Entity Framework Core ile çalışır.

**Protected Members**:
- `_context`: DbContext örneği
- `_set`: DbSet<T> örneği (entity koleksiyonu)

## 📖 Kullanım Örnekleri

### 1. Repository Sınıfı Oluşturma

```csharp
public class UserRepository : Repository<User>
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    // Özel metodlar ekleyebilirsiniz
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await FindFirstAsync(u => u.Email == email);
    }
}
```

### 2. Veri Ekleme

```csharp
// Tek entity ekleme
var user = new User { Name = "Ahmet", Email = "ahmet@example.com" };
await userRepository.CreateAsync(user);
await _context.SaveChangesAsync();

// Çoklu entity ekleme
var users = new List<User> 
{ 
    new User { Name = "Mehmet", Email = "mehmet@example.com" },
    new User { Name = "Fatma", Email = "fatma@example.com" }
};
await userRepository.CreateManyAsync(users);
await _context.SaveChangesAsync();
```

### 3. Veri Sorgulama

```csharp
// Tüm verileri okuma
var allUsers = await userRepository.ReadManyAsync();

// Koşula göre sorgulama
var activeUsers = await userRepository.ReadManyAsync(u => u.IsActive == true);

// İlişkili verileri yükleme (Eager Loading)
var usersWithPosts = await userRepository.ReadManyAsync(
    expression: null,
    includes: new[] { "Posts", "Comments" }
);

// İlk sonucu alma
var firstUser = await userRepository.FindFirstAsync(u => u.Id == 1);

// Primary Key ile sorgulama
var user = await userRepository.ReadByKey(1);
```

### 4. Veri Güncelleme

```csharp
// Tek entity güncelleme
user.Name = "Yeni İsim";
await userRepository.UpdateAsync(user);
await _context.SaveChangesAsync();

// Çoklu entity güncelleme
var users = await userRepository.ReadManyAsync(u => u.IsActive == false);
foreach (var u in users) 
{
    u.LastModified = DateTime.Now;
}
await userRepository.UpdateManyAsync(users);
await _context.SaveChangesAsync();

// Koşula göre güncelleme (Manual)
var usersToUpdate = await userRepository.ReadManyAsync(u => u.Department == "IT");
foreach (var u in usersToUpdate) 
{
    u.Salary += 5000;
}
await userRepository.UpdateManyAsync(usersToUpdate);
await _context.SaveChangesAsync();
```

### 5. Veri Silme

```csharp
// Tek entity silme
await userRepository.DeleteAsync(user);
await _context.SaveChangesAsync();

// Çoklu entity silme
await userRepository.DeleteManyAsync(usersToDelete);
await _context.SaveChangesAsync();

// Koşula göre silme
var inactiveUsers = await userRepository.ReadManyAsync(u => u.IsActive == false);
await userRepository.DeleteManyAsync(inactiveUsers);
await _context.SaveChangesAsync();
```

### 6. Sayma ve Kontrol İşlemleri

```csharp
// Toplam sayı
int totalUsers = await userRepository.CountAsync();

// Koşula göre sayma
int activeCount = await userRepository.CountAsync(u => u.IsActive == true);

// Var mı kontrol
bool hasUsers = await userRepository.AnyAsync();

// Koşula göre var mı kontrol
bool hasAdmins = await userRepository.AnyAsync(u => u.Role == "Admin");
```

## 🔧 API Referansı

### Create (Oluşturma)

| Method | Açıklama |
|--------|----------|
| `CreateAsync(T entity)` | Tek entity ekler |
| `CreateManyAsync(IEnumerable<T> entities)` | Çoklu entity ekler |

### Read (Okuma)

| Method | Açıklama |
|--------|----------|
| `ReadByKey(object entityKey)` | Primary key ile entity getirir |
| `FindFirstAsync(Expression?)` | Koşula göre ilk entity'yi getirir |
| `ReadManyAsync(Expression?, params string[])` | Koşula göre entity'leri getirir, include parametresi ile eager loading yapar |

### Update (Güncelleme)

| Method | Açıklama |
|--------|----------|
| `UpdateAsync(T entity)` | Tek entity günceller |
| `UpdateManyAsync(IEnumerable<T> entities)` | Çoklu entity günceller |
| `UpdateManyAsync(Expression?)` | Koşula göre entity'leri günceller |

### Delete (Silme)

| Method | Açıklama |
|--------|----------|
| `DeleteAsync(T entity)` | Tek entity siler |
| `DeleteManyAsync(IEnumerable<T> entities)` | Çoklu entity siler |
| `DeleteManyAsync(Expression?)` | Koşula göre entity'leri siler |

### Query (Sorgulama)

| Method | Açıklama |
|--------|----------|
| `CountAsync(Expression?)` | Entity sayısını döner |
| `AnyAsync(Expression?)` | Entity var mı kontrol eder |

## 📌 En İyi Uygulamalar

### 1. SaveChanges'i Çağırmayı Unutmayın

Repository işlemlerini çağırdıktan sonra `DbContext.SaveChangesAsync()` mutlaka çağrılmalıdır:

```csharp
await userRepository.CreateAsync(user);
await _context.SaveChangesAsync(); // Zorunlu!
```

### 2. Unit of Work Pattern Kullanın

Birden fazla repository ile çalışırken Unit of Work pattern'ı tercih edin:

```csharp
public class UnitOfWork
{
    private readonly DbContext _context;
    
    public IRepository<User> Users { get; }
    public IRepository<Post> Posts { get; }

    public UnitOfWork(DbContext context)
    {
        _context = context;
        Users = new UserRepository(context);
        Posts = new PostRepository(context);
    }

    public async Task SaveAsync() => await _context.SaveChangesAsync();
}
```

### 3. Eager Loading ile İlişkili Verileri Yükleyin

Çoklu veritabanı çağrılarından kaçınmak için:

```csharp
// Kötü: N+1 query problemi
var users = await userRepository.ReadManyAsync();
foreach (var user in users)
{
    var posts = await postRepository.ReadManyAsync(p => p.UserId == user.Id);
}

// İyi: Tek sorgu
var users = await userRepository.ReadManyAsync(
    includes: new[] { "Posts", "Comments" }
);
```

### 4. Koşul Kontrolleri Yapın

Boş koleksiyonlar ile çalışırken dikkatli olun:

```csharp
var users = await userRepository.ReadManyAsync(u => u.IsActive);
if (users.Any())
{
    // İşlem yap
}
```

### 5. Exception Handling Ekleyin

```csharp
try
{
    var user = new User { Name = "Test", Email = "test@example.com" };
    await userRepository.CreateAsync(user);
    await _context.SaveChangesAsync();
}
catch (DbUpdateException ex)
{
    // Veri tabanı hataları
    logger.LogError(ex, "Veri tabanına kayıt sırasında hata oluştu");
}
```

## ⚠️ Uyarılar

### 1. Expression ile Güncellemeler

Mevcut implementasyonda expression ile düz güncelleme (direct update) yapılamaz:

```csharp
// YAPILMAZ - Her entity'i getirip döngüde günceller
await userRepository.UpdateManyAsync(u => u.IsActive = false);

// YAPILIR - Entity'leri getirip, döngüde güncelleme yapın
var users = await userRepository.ReadManyAsync(u => u.IsActive == true);
foreach (var u in users) u.IsActive = false;
await userRepository.UpdateManyAsync(users);
```

### 2. Include Sınırlaması

Include parametresi string tabanlıdır ve type-safe değildir:

```csharp
// String-based (runtime hatası riski)
var users = await userRepository.ReadManyAsync(
    includes: new[] { "Posts", "Comments" }
);

// Iyileştirme: Strongly-typed include örneği görmek için
// Expression<Func<T, object>> kullanabilirsiniz
```

### 3. Null Expression Kontrolü

Her method'da `?? (x => true)` kontrolü yapılır:

```csharp
// Bu çağrılar eşdeğerdir
await userRepository.ReadManyAsync(); // x => true (tüm veriler)
await userRepository.ReadManyAsync(null); // x => true (tüm veriler)
```

### 4. Asenkron Işlemler

`Task.Run()` kullanılan metodlar gereksiz thread pool işi yaratabilir:

```csharp
// DeleteAsync, UpdateAsync, DeleteManyAsync, UpdateManyAsync
// senkron işlemler olmasına rağmen async sarılmıştır
```

## 🔄 İyileştirme Önerileri

```csharp
// Strongly-typed Include ile geliştirilmiş versiyon
public virtual async Task<IEnumerable<T>> ReadManyAsync(
    Expression<Func<T, bool>>? expression = null,
    params Expression<Func<T, object>>[] includes)
{
    var query = _set.AsQueryable();
    
    foreach (var include in includes)
    {
        query = query.Include(include);
    }
    
    return await query
        .Where(expression ?? (x => true))
        .ToListAsync();
}

// Pagination desteği
public virtual async Task<(IEnumerable<T> data, int total)> 
    ReadManyAsync(int pageNumber, int pageSize, 
    Expression<Func<T, bool>>? expression = null)
{
    var query = _set.Where(expression ?? (x => true));
    var total = await query.CountAsync();
    var data = await query
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    
    return (data, total);
}
```

## 📝 Lisans

Bu kod MIT lisansı altında sunulmuştur.

## 🤝 Katkıda Bulunma

Geliştirme önerileri ve pull request'ler hoş karşılanır.

---

**Son Güncelleme**: 2024
**Sürüm**: 1.0.0
**Framework**: .NET 8.0+
