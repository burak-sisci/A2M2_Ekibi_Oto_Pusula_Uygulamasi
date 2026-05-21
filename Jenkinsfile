pipeline {
    agent any

    environment {
        FRONTEND_IMAGE = 'otopusula-frontend'
        FRONTEND_PORT  = '3000'
    }

    stages {

        // ── 1. GELIŞTIRME: Kodu çek ─────────────────────────────────────
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        // ── 2. DERLEME: Backend (dotnet) ─────────────────────────────────
        stage('Backend - Restore & Build') {
            steps {
                dir('backend/backend.API') {
                    bat 'dotnet restore'
                    bat 'dotnet build --configuration Release --no-restore'
                }
            }
        }

        // ── 3. TEST: Backend unit testleri ───────────────────────────────
        stage('Backend - Test') {
            steps {
                dir('backend/backend.API') {
                    bat 'dotnet test --no-build --configuration Release --verbosity normal'
                }
            }
            post {
                failure {
                    echo 'Backend testleri basarisiz - pipeline durduruldu.'
                }
            }
        }

        // ── 4. DERLEME: Frontend (npm) ───────────────────────────────────
        stage('Frontend - Install & Build') {
            steps {
                dir('frontend') {
                    bat 'npm ci'
                    bat 'npm run build'
                }
            }
        }

        // ── 5. PAKETLEME: Frontend Docker image olustur ──────────────────
        //    (Backend image'i docker-compose --build ile olusturulacak)
        stage('Docker Build - Frontend') {
            steps {
                dir('frontend') {
                    bat "docker build -t %FRONTEND_IMAGE%:latest -t %FRONTEND_IMAGE%:%BUILD_NUMBER% ."
                }
            }
        }

        // ── 6. DAGITIM: Tüm servisleri Docker üzerinden ayağa kaldır ─────
        stage('Deploy - Docker Compose (Backend + Infra)') {
            steps {
                dir('backend') {
                    // Onceki container'lari durdur, yeniden olustur
                    bat 'docker compose down --remove-orphans'
                    // Backend image'ini de yeniden build ederek baslat
                    bat 'docker compose up -d --build'
                }
            }
        }

        stage('Deploy - Docker Run (Frontend)') {
            steps {
                // Onceki frontend container varsa temizle
                bat """
                    docker stop %FRONTEND_IMAGE% 2>nul || echo frontend container yoktu
                    docker rm   %FRONTEND_IMAGE% 2>nul || echo frontend container yoktu
                """
                // Frontend container'i baslat
                bat "docker run -d --name %FRONTEND_IMAGE% -p %FRONTEND_PORT%:80 --network backend_otopusula-net %FRONTEND_IMAGE%:latest"
            }
            post {
                success {
                    echo "Deployment basarili!"
                    echo "Backend  -> http://localhost:8081"
                    echo "Frontend -> http://localhost:${FRONTEND_PORT}"
                }
            }
        }

    }

    post {
        success {
            echo "Pipeline BASARILI tamamlandi. Build: #${BUILD_NUMBER}"
        }
        failure {
            echo "Pipeline BASARISIZ oldu. Jenkins loglarini kontrol edin."
        }
        always {
            cleanWs()
        }
    }
}
