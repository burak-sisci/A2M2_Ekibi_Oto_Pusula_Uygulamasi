import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext.jsx';
import { authAPI } from '../../services/api.js';
import './AuthForm.css';

function Login() {
    const [formData, setFormData] = useState({ email: '', password: '' });
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const { login } = useAuth();
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');
        try {
            const { data } = await authAPI.login(formData);
            login(data);
            navigate('/');
        } catch (err) {
            setError(err.response?.data?.message || 'Giriş yapılırken bir hata oluştu');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="auth-container">
            <div className="auth-card">
                <h1 className="auth-title">Giriş Yap</h1>
                <p className="auth-subtitle">A2M2 hesabınıza giriş yapın</p>
                {error && <div className="auth-error">{error}</div>}
                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label htmlFor="email">E-posta</label>
                        <input id="email" type="email" className="form-control" placeholder="ornek@email.com"
                            value={formData.email} onChange={(e) => setFormData({ ...formData, email: e.target.value })} required />
                    </div>
                    <div className="form-group">
                        <label htmlFor="password">Şifre</label>
                        <input id="password" type="password" className="form-control" placeholder="••••••••"
                            value={formData.password} onChange={(e) => setFormData({ ...formData, password: e.target.value })} required />
                    </div>
                    <button type="submit" className="btn btn-primary auth-btn" disabled={loading}>
                        {loading ? 'Giriş yapılıyor...' : 'Giriş Yap'}
                    </button>
                </form>
                <p className="auth-link">
                    Hesabınız yok mu? <Link to="/register">Kayıt Ol</Link>
                </p>
            </div>
        </div>
    );
}

export default Login;
