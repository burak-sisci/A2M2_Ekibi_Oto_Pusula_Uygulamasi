import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext.jsx';
import { authAPI } from '../../services/api.js';
import './AuthForm.css';

function Register() {
    const [formData, setFormData] = useState({ name: '', email: '', password: '', confirmPassword: '' });
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const { login } = useAuth();
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (formData.password !== formData.confirmPassword) {
            return setError('Şifreler eşleşmiyor');
        }
        setLoading(true);
        setError('');
        try {
            const { data } = await authAPI.register({
                name: formData.name,
                email: formData.email,
                password: formData.password,
            });
            login(data);
            navigate('/');
        } catch (err) {
            setError(err.response?.data?.message || 'Kayıt olurken bir hata oluştu');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="auth-container">
            <div className="auth-card">
                <h1 className="auth-title">Kayıt Ol</h1>
                <p className="auth-subtitle">A2M2'ye ücretsiz üye olun</p>
                {error && <div className="auth-error">{error}</div>}
                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label htmlFor="name">Ad Soyad</label>
                        <input id="name" type="text" className="form-control" placeholder="Ad Soyad"
                            value={formData.name} onChange={(e) => setFormData({ ...formData, name: e.target.value })} required />
                    </div>
                    <div className="form-group">
                        <label htmlFor="email">E-posta</label>
                        <input id="email" type="email" className="form-control" placeholder="ornek@email.com"
                            value={formData.email} onChange={(e) => setFormData({ ...formData, email: e.target.value })} required />
                    </div>
                    <div className="form-group">
                        <label htmlFor="password">Şifre</label>
                        <input id="password" type="password" className="form-control" placeholder="En az 6 karakter"
                            value={formData.password} onChange={(e) => setFormData({ ...formData, password: e.target.value })} required minLength={6} />
                    </div>
                    <div className="form-group">
                        <label htmlFor="confirmPassword">Şifre Tekrar</label>
                        <input id="confirmPassword" type="password" className="form-control" placeholder="Şifrenizi tekrar girin"
                            value={formData.confirmPassword} onChange={(e) => setFormData({ ...formData, confirmPassword: e.target.value })} required />
                    </div>
                    <button type="submit" className="btn btn-primary auth-btn" disabled={loading}>
                        {loading ? 'Kayıt yapılıyor...' : 'Kayıt Ol'}
                    </button>
                </form>
                <p className="auth-link">
                    Zaten hesabınız var mı? <Link to="/login">Giriş Yap</Link>
                </p>
            </div>
        </div>
    );
}

export default Register;
