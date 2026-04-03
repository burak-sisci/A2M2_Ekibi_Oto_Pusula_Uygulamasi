import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { useTheme } from '../../context/ThemeContext';
import { logout } from '../../api/authApi';
import { toast } from 'react-toastify';

const Navbar = () => {
  const { user, logoutUser } = useAuth();
  const { dark, toggleTheme } = useTheme();
  const navigate = useNavigate();
  const [mobileOpen, setMobileOpen] = useState(false);

  const handleLogout = async () => {
    try { await logout(); } catch { /* token expired */ }
    logoutUser();
    toast.success('Basariyla cikis yapildi');
    navigate('/');
  };

  const closeMobile = () => setMobileOpen(false);

  return (
    <nav className="sticky top-0 z-50 glass-panel shadow-sm dark:bg-slate-900/80 dark:border-b dark:border-slate-700/50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex items-center justify-between h-16">
          {/* Logo */}
          <Link to="/" className="flex items-center gap-2 text-xl font-bold" onClick={closeMobile}>
            <span className="text-primary-600 dark:text-primary-400">OTO</span>
            <span className="text-primary-900 dark:text-white">PUSULA</span>
          </Link>

          {/* Desktop Menu */}
          <div className="hidden md:flex items-center gap-1">
            <Link to="/cars" className="px-4 py-2 rounded-lg text-sm font-medium text-slate-600 dark:text-slate-300 hover:text-primary-600 dark:hover:text-primary-400 hover:bg-primary-50 dark:hover:bg-slate-800 transition-all flex items-center gap-2">
              <i className="ri-car-line"></i> Ilanlar
            </Link>
            <Link to="/prediction" className="px-4 py-2 rounded-lg text-sm font-medium text-slate-600 dark:text-slate-300 hover:text-primary-600 dark:hover:text-primary-400 hover:bg-primary-50 dark:hover:bg-slate-800 transition-all flex items-center gap-2">
              <i className="ri-line-chart-line"></i> Fiyat Tahmini
            </Link>

            {user ? (
              <>
                <Link to="/cars/new" className="px-4 py-2 rounded-lg text-sm font-medium bg-primary-600 text-white hover:bg-primary-700 transition-all flex items-center gap-2 shadow-sm">
                  <i className="ri-add-line"></i> Ilan Ver
                </Link>
                <Link to="/lists" className="px-4 py-2 rounded-lg text-sm font-medium text-slate-600 dark:text-slate-300 hover:text-primary-600 dark:hover:text-primary-400 hover:bg-primary-50 dark:hover:bg-slate-800 transition-all flex items-center gap-2">
                  <i className="ri-heart-line"></i> Listelerim
                </Link>
                <Link to="/profile" className="px-4 py-2 rounded-lg text-sm font-medium text-slate-600 dark:text-slate-300 hover:text-primary-600 dark:hover:text-primary-400 hover:bg-primary-50 dark:hover:bg-slate-800 transition-all flex items-center gap-2">
                  <i className="ri-user-line"></i> Profil
                </Link>
                <button onClick={handleLogout} className="px-4 py-2 rounded-lg text-sm font-medium text-slate-500 dark:text-slate-400 hover:text-red-600 hover:bg-red-50 dark:hover:bg-red-900/20 transition-all flex items-center gap-2">
                  <i className="ri-logout-box-r-line"></i> Cikis
                </button>
              </>
            ) : (
              <>
                <Link to="/login" className="px-4 py-2 rounded-lg text-sm font-medium text-slate-600 dark:text-slate-300 hover:text-primary-600 dark:hover:text-primary-400 hover:bg-primary-50 dark:hover:bg-slate-800 transition-all flex items-center gap-2">
                  <i className="ri-login-box-line"></i> Giris Yap
                </Link>
                <Link to="/register" className="px-4 py-2 rounded-lg text-sm font-medium bg-primary-600 text-white hover:bg-primary-700 transition-all flex items-center gap-2 shadow-sm">
                  Kayit Ol
                </Link>
              </>
            )}

            {/* Theme Toggle */}
            <button onClick={toggleTheme} className="ml-2 p-2 rounded-lg text-slate-500 dark:text-slate-400 hover:bg-slate-100 dark:hover:bg-slate-800 transition-all" title={dark ? 'Acik Tema' : 'Karanlik Tema'}>
              <i className={dark ? 'ri-sun-line text-lg' : 'ri-moon-line text-lg'}></i>
            </button>
          </div>

          {/* Mobile Right */}
          <div className="flex md:hidden items-center gap-1">
            <button onClick={toggleTheme} className="p-2 rounded-lg text-slate-500 dark:text-slate-400 hover:bg-slate-100 dark:hover:bg-slate-800 transition-all">
              <i className={dark ? 'ri-sun-line text-lg' : 'ri-moon-line text-lg'}></i>
            </button>
            <button onClick={() => setMobileOpen(!mobileOpen)} className="p-2 rounded-lg text-slate-600 dark:text-slate-300 hover:bg-slate-100 dark:hover:bg-slate-800 transition-all">
              <i className={mobileOpen ? 'ri-close-line text-xl' : 'ri-menu-line text-xl'}></i>
            </button>
          </div>
        </div>

        {/* Mobile Menu */}
        {mobileOpen && (
          <div className="md:hidden pb-4 border-t border-slate-200/60 dark:border-slate-700/60 mt-2 pt-3 flex flex-col gap-1">
            <Link to="/cars" onClick={closeMobile} className="px-4 py-3 rounded-lg text-sm font-medium text-slate-600 dark:text-slate-300 hover:bg-primary-50 dark:hover:bg-slate-800 hover:text-primary-600 dark:hover:text-primary-400 transition-all flex items-center gap-3">
              <i className="ri-car-line text-lg"></i> Ilanlar
            </Link>
            <Link to="/prediction" onClick={closeMobile} className="px-4 py-3 rounded-lg text-sm font-medium text-slate-600 dark:text-slate-300 hover:bg-primary-50 dark:hover:bg-slate-800 hover:text-primary-600 dark:hover:text-primary-400 transition-all flex items-center gap-3">
              <i className="ri-line-chart-line text-lg"></i> Fiyat Tahmini
            </Link>
            {user ? (
              <>
                <Link to="/cars/new" onClick={closeMobile} className="px-4 py-3 rounded-lg text-sm font-medium bg-primary-600 text-white text-center">
                  <i className="ri-add-line"></i> Ilan Ver
                </Link>
                <Link to="/lists" onClick={closeMobile} className="px-4 py-3 rounded-lg text-sm font-medium text-slate-600 dark:text-slate-300 hover:bg-primary-50 dark:hover:bg-slate-800 flex items-center gap-3">
                  <i className="ri-heart-line text-lg"></i> Listelerim
                </Link>
                <Link to="/profile" onClick={closeMobile} className="px-4 py-3 rounded-lg text-sm font-medium text-slate-600 dark:text-slate-300 hover:bg-primary-50 dark:hover:bg-slate-800 flex items-center gap-3">
                  <i className="ri-user-line text-lg"></i> Profil
                </Link>
                <button onClick={() => { handleLogout(); closeMobile(); }} className="px-4 py-3 rounded-lg text-sm font-medium text-red-500 hover:bg-red-50 dark:hover:bg-red-900/20 text-left flex items-center gap-3">
                  <i className="ri-logout-box-r-line text-lg"></i> Cikis
                </button>
              </>
            ) : (
              <>
                <Link to="/login" onClick={closeMobile} className="px-4 py-3 rounded-lg text-sm font-medium text-slate-600 dark:text-slate-300 hover:bg-primary-50 dark:hover:bg-slate-800 flex items-center gap-3">
                  <i className="ri-login-box-line text-lg"></i> Giris Yap
                </Link>
                <Link to="/register" onClick={closeMobile} className="px-4 py-3 rounded-lg text-sm font-medium bg-primary-600 text-white text-center">
                  Kayit Ol
                </Link>
              </>
            )}
          </div>
        )}
      </div>
    </nav>
  );
};

export default Navbar;
