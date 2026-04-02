import { Link } from 'react-router-dom';

const Footer = () => (
  <footer className="bg-primary-950 text-white mt-auto">
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
        {/* Brand */}
        <div>
          <div className="text-2xl font-bold mb-3">
            <span className="text-primary-400">OTO</span>
            <span className="text-white"> PUSULA</span>
          </div>
          <p className="text-slate-400 text-sm leading-relaxed">
            Aracini degerlendir, dogru fiyati bul. Yapay zeka destekli fiyat tahmini ile guvenli arac alim-satim platformu.
          </p>
          <div className="flex gap-3 mt-4">
            <a href="#" className="w-9 h-9 rounded-lg bg-white/10 hover:bg-primary-600 flex items-center justify-center transition-all">
              <i className="ri-facebook-fill text-sm"></i>
            </a>
            <a href="#" className="w-9 h-9 rounded-lg bg-white/10 hover:bg-primary-600 flex items-center justify-center transition-all">
              <i className="ri-twitter-x-fill text-sm"></i>
            </a>
            <a href="#" className="w-9 h-9 rounded-lg bg-white/10 hover:bg-primary-600 flex items-center justify-center transition-all">
              <i className="ri-instagram-fill text-sm"></i>
            </a>
          </div>
        </div>

        {/* Quick Links */}
        <div>
          <h4 className="font-semibold text-white mb-4">Hizli Erisim</h4>
          <ul className="space-y-2">
            <li><Link to="/" className="text-slate-400 hover:text-primary-400 text-sm transition-colors">Anasayfa</Link></li>
            <li><Link to="/cars" className="text-slate-400 hover:text-primary-400 text-sm transition-colors">Arac Ilanlari</Link></li>
            <li><Link to="/prediction" className="text-slate-400 hover:text-primary-400 text-sm transition-colors">Fiyat Tahmini</Link></li>
            <li><Link to="/cars/new" className="text-slate-400 hover:text-primary-400 text-sm transition-colors">Ilan Ver</Link></li>
          </ul>
        </div>

        {/* Help */}
        <div>
          <h4 className="font-semibold text-white mb-4">Yardim</h4>
          <ul className="space-y-2">
            <li><a href="#" className="text-slate-400 hover:text-primary-400 text-sm transition-colors">Sikca Sorulan Sorular</a></li>
            <li><a href="#" className="text-slate-400 hover:text-primary-400 text-sm transition-colors">Kullanim Kosullari</a></li>
            <li><a href="#" className="text-slate-400 hover:text-primary-400 text-sm transition-colors">Gizlilik Politikasi</a></li>
          </ul>
        </div>

        {/* Contact */}
        <div>
          <h4 className="font-semibold text-white mb-4">Iletisim</h4>
          <ul className="space-y-3">
            <li className="flex items-start gap-3 text-slate-400 text-sm">
              <i className="ri-map-pin-line text-primary-400 mt-0.5"></i>
              {/* TODO: Adres bilgisi eklenecek */}
              <span>Adres bilgisi eklenecek</span>
            </li>
            <li className="flex items-center gap-3 text-slate-400 text-sm">
              <i className="ri-phone-line text-primary-400"></i>
              {/* TODO: Telefon numarasi eklenecek */}
              <span>Telefon eklenecek</span>
            </li>
            <li className="flex items-center gap-3 text-slate-400 text-sm">
              <i className="ri-mail-line text-primary-400"></i>
              {/* TODO: Email adresi eklenecek */}
              <span>info@otopusula.com</span>
            </li>
          </ul>
        </div>
      </div>
    </div>

    {/* Bottom Bar */}
    <div className="border-t border-white/10">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex flex-col sm:flex-row items-center justify-between gap-2">
        <p className="text-slate-500 text-xs">&copy; 2026 Oto Pusula. Tum haklari saklidir.</p>
        <p className="text-slate-600 text-xs">Yapay zeka destekli arac degerleme platformu</p>
      </div>
    </div>
  </footer>
);

export default Footer;
