# CI/CD — GitHub Actions Pipelines

Bu proje **üç ayrı workflow** ile GitHub Actions üzerinde CI çalıştırır. Her workflow yalnızca kendi servisinin dosyaları değişince tetiklenir (`paths:` filter).

| Workflow | Trigger | Çıktı |
|----------|---------|-------|
| **Backend CI** (`backend.yml`) | `backend/**` | `dotnet build` (Release) |
| **Notification Service CI** (`notification.yml`) | `notification-service/**` | `dotnet build` (Release) |
| **Mobile CI** (`mobile.yml`) | `otopusula_mobile/**` | `flutter analyze` + `flutter test` + debug APK artifact |

> Deploy adımı **yok** — Faz 3 (Fly.io) ücretsiz tier maliyet endişesiyle atlandı. Workflow'lar sadece build ve test'i otomatize eder. APK GitHub Actions artifact'ı olarak indirilebilir.

---

## Gerekli GitHub Secrets

Mobil workflow Firebase config'ine ihtiyaç duyar. Repo sahibinin **bir kez** ayarlaması gereken secret:

| Secret Adı | Nasıl Üretilir | Açıklama |
|------------|----------------|----------|
| `GOOGLE_SERVICES_JSON_B64` | `[Convert]::ToBase64String([IO.File]::ReadAllBytes("otopusula_mobile/android/app/google-services.json"))` | Firebase Android config — runner'da `android/app/google-services.json`'a decode edilir |

### Eklemek için

1. GitHub repo sayfası → **Settings** → **Secrets and variables** → **Actions**.
2. **"New repository secret"** butonu.
3. Name: `GOOGLE_SERVICES_JSON_B64`, Secret: yukarıdaki PowerShell komutunun çıktısı (uzun base64 string).
4. **Add secret**.

### Variables (opsiyonel)

| Variable | Default | Açıklama |
|----------|---------|----------|
| `API_BASE_URL` | `http://10.0.2.2:8080` | APK build'inde `--dart-define=API_BASE_URL=...` olarak verilir. Canlıda Fly.io URL'i veya tunel adresi ile değiştirilebilir. |

Variables aynı menüde **"Variables"** sekmesinden eklenir (secrets'tan farklı — log'da görünür, hassas olmayanlar için).

---

## Workflow akışları

### Backend CI
```
checkout → setup-dotnet (10.0.x preview) → NuGet cache → restore → build (Release)
```
Test projesi henüz yoksa `dotnet test` adımı yorum satırında bekler.

### Notification Service CI
Backend ile aynı yapı, sadece `working-directory: notification-service`.

### Mobile CI
```
checkout → setup-java 17 → setup-flutter (stable) → decode google-services.json → pub get
         → flutter analyze → flutter test → flutter build apk --debug
         → upload artifact (otopusula-debug-apk, 30 gün)
```

APK indirmek için: GitHub repo → **Actions** sekmesi → ilgili workflow run → **Artifacts** bölümü.

---

## Yerel test

Workflow YAML'lerini lokalde çalıştırmak için [`act`](https://github.com/nektos/act) kullanılabilir:

```powershell
act -j build -W .github/workflows/backend.yml
```

Mobile workflow için Docker üzerinde Android SDK büyük olduğu için lokalde önerilmez — sadece push edip GitHub Actions'ta çalıştırın.

---

## Branch korunması (öneri)

Repo ayarları → **Branches** → **Add branch protection rule**:
- Branch name pattern: `main`
- **Require status checks to pass before merging** ✅
  - Required checks: `Backend CI / build`, `Notification Service CI / build`, `Mobile CI / analyze-and-build`
- **Require pull request reviews before merging** ✅ (en az 1 reviewer)

Bu sayede `main` branch'e doğrudan push engellenir; tüm değişiklikler PR + green CI sonrası birleştirilir.
