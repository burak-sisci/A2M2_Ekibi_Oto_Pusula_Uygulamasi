import { Link } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext.jsx';
import { authAPI } from '../../services/api.js';
import './Layout.css';

function Navbar() {
    const { user, isAuthenticated, logout } = useAuth();

    const handleLogout = async () => {
        try {
            await authAPI.logout();
        } catch {
            // Ignore
        }
        logout();
    };

    return (
        <nav className="navbar">
            <div className="navbar-container">
                <Link to="/" className="navbar-brand">
                    <span className="brand-icon">🚗</span>
                    <span className="brand-text">A2M2</span>
                </Link>

                <div className="navbar-links">
                    <Link to="/" className="nav-link">İlanlar</Link>
                    <Link to="/price-estimator" className="nav-link">Fiyat Tahmini</Link>

                    {isAuthenticated ? (
                        <>
                            <Link to="/create-listing" className="nav-link">İlan Ver</Link>
                            <Link to="/favorites" className="nav-link">Favorilerim</Link>
                            <div className="nav-user">
                                <span className="nav-user-name">{user?.name}</span>
                                <button className="btn btn-secondary btn-sm" onClick={handleLogout}>Çıkış</button>
                            </div>
                        </>
                    ) : (
                        <div className="nav-auth">
                            <Link to="/login" className="btn btn-secondary btn-sm">Giriş</Link>
                            <Link to="/register" className="btn btn-primary btn-sm">Kayıt Ol</Link>
                        </div>
                    )}
                </div>
            </div>
        </nav>
    );
}

export default Navbar;
