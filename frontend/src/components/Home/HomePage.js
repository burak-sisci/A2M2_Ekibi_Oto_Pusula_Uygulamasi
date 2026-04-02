import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { getCars } from '../../api/carApi';

const formatPrice = (price) => new Intl.NumberFormat('tr-TR').format(price) + ' TL';
const formatKm = (km) => new Intl.NumberFormat('tr-TR').format(km) + ' km';

const HomePage = () => {
  const [popularCars, setPopularCars] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchPopular = async () => {
      try {
        const res = await getCars({ limit: 8, offset: 0 });
        setPopularCars(res.data.data || []);
      } catch { /* ignore */ }
      finally { setLoading(false); }
    };
    fetchPopular();
  }, []);

  return (
    <div className="min-h-screen">
      {/* ==================== HERO SECTION ==================== */}
      <section className="hero-bg relative min-h-[600px] lg:min-h-[700px] flex items-center overflow-hidden"
        style={{
          backgroundImage: "url('/images/hero-bg.jpg')",
          backgroundSize: 'cover',
          backgroundPosition: 'center'
        }}>
        {/* Animated blobs */}
        <div className="absolute inset-0 overflow-hidden">
          <div className="absolute -top-40 -right-40 w-80 h-80 bg-blue-300/20 rounded-full mix-blend-multiply filter blur-xl animate-blob"></div>
          <div className="absolute -bottom-40 -left-40 w-80 h-80 bg-indigo-300/20 rounded-full mix-blend-multiply filter blur-xl animate-blob animation-delay-2000"></div>
          <div className="absolute top-1/2 left-1/2 w-80 h-80 bg-sky-300/20 rounded-full mix-blend-multiply filter blur-xl animate-blob animation-delay-4000"></div>
        </div>

        <div className="relative z-10 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-20">
          <div className="max-w-3xl">
            <div className="inline-flex items-center gap-2 bg-white/10 backdrop-blur-sm text-white/90 text-sm font-medium px-4 py-2 rounded-full mb-6 border border-white/20">
              <i className="ri-sparkle-line"></i>
              Yapay Zeka Destekli Fiyat Tahmini
            </div>
            <h1 className="text-4xl sm:text-5xl lg:text-6xl font-bold text-white leading-tight mb-6">
              Aracini Degerlendir,
              <span className="block text-transparent bg-clip-text bg-gradient-to-r from-blue-200 to-sky-200">
                Dogru Fiyati Bul
              </span>
            </h1>
            <p className="text-lg sm:text-xl text-blue-100/80 leading-relaxed mb-8 max-w-2xl">
              Binlerce ilan arasinda en uygun araci kesfet. Yapay zeka destekli fiyat tahmini ile aracinin gercek degerini ogren.
            </p>
            <div className="flex flex-wrap gap-4">
              <Link to="/prediction" className="inline-flex items-center gap-2 bg-white text-primary-700 font-semibold px-6 py-3 rounded-xl hover:bg-blue-50 transition-all shadow-lg hover:shadow-xl">
                <i className="ri-line-chart-line text-lg"></i>
                Fiyat Tahmini Yap
              </Link>
              <Link to="/cars" className="inline-flex items-center gap-2 bg-white/10 backdrop-blur-sm text-white font-semibold px-6 py-3 rounded-xl border border-white/30 hover:bg-white/20 transition-all">
                <i className="ri-car-line text-lg"></i>
                Ilanlari Incele
              </Link>
            </div>
          </div>
        </div>

        {/* Hero car image - right side */}
        <div className="hidden xl:block absolute right-0 top-1/2 -translate-y-1/2 w-[500px] h-[400px] z-10">
          <img src="/images/hero-car.png" alt="Oto Pusula" className="w-full h-full object-contain drop-shadow-2xl" />
        </div>
      </section>

      {/* ==================== FIYAT TAHMINI SECTION ==================== */}
      <section className="py-16 lg:py-20 bg-white dark:bg-slate-900 relative overflow-hidden">
        {/* BG decoration */}
        <div className="absolute top-0 right-0 w-96 h-96 bg-primary-50 dark:bg-primary-900/20 rounded-full -translate-y-1/2 translate-x-1/2 blur-3xl"></div>

        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 relative">
          <div className="grid lg:grid-cols-2 gap-12 items-center">
            {/* Left - Content */}
            <div>
              <div className="inline-flex items-center gap-2 bg-primary-50 dark:bg-primary-900/30 text-primary-700 dark:text-primary-300 text-sm font-semibold px-4 py-2 rounded-full mb-4">
                <i className="ri-robot-line"></i>
                AI Destekli
              </div>
              <h2 className="text-3xl sm:text-4xl font-bold text-slate-900 dark:text-white mb-4 leading-tight">
                Yapay Zeka ile<br />
                <span className="text-primary-600 dark:text-primary-400">Arac Fiyat Tahmini</span>
              </h2>
              <p className="text-slate-600 dark:text-slate-400 leading-relaxed mb-6">
                Makine ogrenmesi modeli ile aracinin piyasa degerini aninda ogren. Marka, model, yil, kilometre ve daha bircok parametre ile hassas fiyat tahmini.
              </p>
              <ul className="space-y-3 mb-8">
                {[
                  'Binlerce gercek ilan verisinden egitilmis model',
                  '26 farkli parametre ile hassas analiz',
                  'Boya, hasar ve degisen parca etkisi hesaplama',
                  'Aninda sonuc, ucretsiz kullanim'
                ].map((item, i) => (
                  <li key={i} className="flex items-center gap-3 text-slate-700 dark:text-slate-300">
                    <div className="w-6 h-6 rounded-full bg-green-100 dark:bg-green-900/30 flex items-center justify-center flex-shrink-0">
                      <i className="ri-check-line text-green-600 text-sm"></i>
                    </div>
                    {item}
                  </li>
                ))}
              </ul>
              <Link to="/prediction" className="inline-flex items-center gap-2 bg-primary-600 text-white font-semibold px-6 py-3 rounded-xl hover:bg-primary-700 transition-all shadow-md hover:shadow-lg">
                <i className="ri-line-chart-line"></i>
                Hemen Degerle
                <i className="ri-arrow-right-line"></i>
              </Link>
            </div>

            {/* Right - Image placeholder */}
            <div className="relative">
              <div className="rounded-2xl overflow-hidden shadow-2xl">
                <img src="/images/prediction-mockup.png" alt="Fiyat Tahmini" className="w-full h-auto object-cover" />
              </div>
              {/* Floating badge */}
              <div className="absolute -bottom-4 -left-4 bg-white dark:bg-slate-800 rounded-xl shadow-lg p-4 flex items-center gap-3">
                <div className="w-10 h-10 bg-green-100 dark:bg-green-900/30 rounded-full flex items-center justify-center">
                  <i className="ri-check-double-line text-green-600"></i>
                </div>
                <div>
                  <p className="text-sm font-semibold text-slate-900 dark:text-white">%92 Dogruluk</p>
                  <p className="text-xs text-slate-500 dark:text-slate-400">R2 Skor Orani</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* ==================== NEDEN OTO PUSULA ==================== */}
      <section className="py-16 lg:py-20 bg-slate-50 dark:bg-slate-800/50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-12">
            <h2 className="text-3xl font-bold text-slate-900 dark:text-white mb-3">Neden Oto Pusula?</h2>
            <p className="text-slate-600 dark:text-slate-400 max-w-2xl mx-auto">Guvenli arac alim-satim deneyimi icin ihtiyaciniz olan her sey</p>
          </div>
          <div className="grid md:grid-cols-3 gap-8">
            {[
              {
                icon: 'ri-robot-line',
                title: 'AI Fiyat Tahmini',
                desc: 'Yapay zeka modeli ile aracinin gercek piyasa degerini aninda ogren. Gizli ucret yok, tamamen ucretsiz.'
              },
              {
                icon: 'ri-shield-check-line',
                title: 'Guvenilir Platform',
                desc: 'Dogrulanmis ilan sahipleri, detayli arac bilgileri ve seffaf fiyatlandirma ile guvenle alisveris.'
              },
              {
                icon: 'ri-speed-up-line',
                title: 'Hizli ve Kolay',
                desc: 'Aracini dakikalar icinde ilan ver veya binlerce ilan arasinda kolayca filtrele ve bul.'
              }
            ].map((feature, i) => (
              <div key={i} className="glass-card rounded-2xl p-8 text-center group">
                <div className="w-16 h-16 bg-primary-50 dark:bg-primary-900/30 rounded-2xl flex items-center justify-center mx-auto mb-5 group-hover:bg-primary-100 dark:group-hover:bg-primary-900/50 transition-all">
                  <i className={`${feature.icon} text-2xl text-primary-600`}></i>
                </div>
                <h3 className="text-lg font-bold text-slate-900 dark:text-white mb-3">{feature.title}</h3>
                <p className="text-slate-600 dark:text-slate-400 text-sm leading-relaxed">{feature.desc}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* ==================== POPULER ILANLAR ==================== */}
      <section className="py-16 lg:py-20 bg-white dark:bg-slate-900">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex items-center justify-between mb-10">
            <div>
              <h2 className="text-3xl font-bold text-slate-900 dark:text-white mb-2">Populer Ilanlar</h2>
              <p className="text-slate-600 dark:text-slate-400">En son eklenen arac ilanlari</p>
            </div>
            <Link to="/cars" className="hidden sm:inline-flex items-center gap-2 text-primary-600 font-semibold hover:text-primary-700 transition-colors">
              Tum Ilanlar <i className="ri-arrow-right-line"></i>
            </Link>
          </div>

          {loading ? (
            <div className="flex items-center justify-center py-20">
              <div className="w-10 h-10 border-4 border-primary-200 border-t-primary-600 rounded-full animate-spin"></div>
            </div>
          ) : popularCars.length === 0 ? (
            <div className="text-center py-20 text-slate-500">
              <i className="ri-car-line text-4xl mb-3 block"></i>
              <p>Henuz ilan eklenmemis</p>
            </div>
          ) : (
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
              {popularCars.map((car) => (
                <Link key={car.id} to={`/cars/${car.id}`} className="group bg-white dark:bg-slate-800 rounded-2xl border border-slate-100 dark:border-slate-700 overflow-hidden hover:shadow-xl hover:-translate-y-1 transition-all duration-300">
                  {/* Image */}
                  <div className="relative h-48 overflow-hidden bg-slate-100">
                    {car.resimler && car.resimler.length > 0 ? (
                      <img src={car.resimler[0]} alt={`${car.marka} ${car.seri}`} className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500" />
                    ) : (
                      <div className="w-full h-full img-placeholder">
                        <i className="ri-car-line text-3xl text-slate-300"></i>
                      </div>
                    )}
                    {car.agirHasarKaydi && (
                      <span className="absolute top-3 left-3 bg-red-500 text-white text-xs font-semibold px-2.5 py-1 rounded-lg">Agir Hasar</span>
                    )}
                    {car.takasaUygun && (
                      <span className="absolute top-3 right-3 bg-emerald-500 text-white text-xs font-semibold px-2.5 py-1 rounded-lg">Takas</span>
                    )}
                  </div>
                  {/* Body */}
                  <div className="p-4">
                    <h3 className="font-bold text-slate-900 dark:text-white mb-1 group-hover:text-primary-600 dark:group-hover:text-primary-400 transition-colors truncate">
                      {car.marka} {car.seri} {car.model}
                    </h3>
                    <p className="text-lg font-bold text-primary-600 dark:text-primary-400 mb-3">{formatPrice(car.fiyat)}</p>
                    <div className="grid grid-cols-2 gap-2 text-xs text-slate-500 dark:text-slate-400">
                      <span className="flex items-center gap-1.5"><i className="ri-calendar-line"></i> {car.yil}</span>
                      <span className="flex items-center gap-1.5"><i className="ri-road-map-line"></i> {formatKm(car.kilometre)}</span>
                      <span className="flex items-center gap-1.5"><i className="ri-gas-station-line"></i> {car.yakitTipi}</span>
                      <span className="flex items-center gap-1.5"><i className="ri-steering-2-line"></i> {car.vitesTipi}</span>
                    </div>
                  </div>
                </Link>
              ))}
            </div>
          )}

          <div className="text-center mt-10 sm:hidden">
            <Link to="/cars" className="inline-flex items-center gap-2 bg-primary-600 text-white font-semibold px-6 py-3 rounded-xl hover:bg-primary-700 transition-all">
              Tum Ilanlari Gor <i className="ri-arrow-right-line"></i>
            </Link>
          </div>
        </div>
      </section>

      {/* ==================== CTA SECTION ==================== */}
      <section className="py-16 lg:py-20 relative overflow-hidden"
        style={{
          backgroundImage: "url('/images/cta-bg.jpg')",
          backgroundSize: 'cover',
          backgroundPosition: 'center'
        }}>
        <div className="absolute inset-0 bg-black/20"></div>
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 text-center relative z-10">
          <h2 className="text-3xl sm:text-4xl font-bold text-white mb-4">Aracini Satmak mi Istiyorsun?</h2>
          <p className="text-blue-100/80 text-lg mb-8 max-w-2xl mx-auto">
            Ucretsiz ilan ver, binlerce aliciya ulas. Yapay zeka ile aracinin degerini belirle.
          </p>
          <div className="flex flex-wrap justify-center gap-4">
            <Link to="/cars/new" className="inline-flex items-center gap-2 bg-white text-primary-700 font-semibold px-6 py-3 rounded-xl hover:bg-blue-50 transition-all shadow-lg">
              <i className="ri-add-line"></i> Ucretsiz Ilan Ver
            </Link>
            <Link to="/prediction" className="inline-flex items-center gap-2 bg-white/10 text-white font-semibold px-6 py-3 rounded-xl border border-white/30 hover:bg-white/20 transition-all">
              <i className="ri-line-chart-line"></i> Fiyat Tahmini Yap
            </Link>
          </div>
        </div>
      </section>
    </div>
  );
};

export default HomePage;
