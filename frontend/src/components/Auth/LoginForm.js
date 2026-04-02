import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { login } from '../../api/authApi';
import { useAuth } from '../../context/AuthContext';
import { toast } from 'react-toastify';
import './Auth.css';

const LoginForm = () => {
  const [form, setForm] = useState({ email: '', password: '' });
  const [loading, setLoading] = useState(false);
  const { loginUser } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      const res = await login(form);
      loginUser(res.data.token);
      toast.success('Giris basarili!');
      navigate('/');
    } catch (err) {
      toast.error(err.response?.data?.message || 'Giris basarisiz');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-container">
      <form className="auth-form" onSubmit={handleSubmit}>
        <h2>Giris Yap</h2>
        <div className="form-group">
          <label>Email</label>
          <input
            type="email"
            value={form.email}
            onChange={(e) => setForm({ ...form, email: e.target.value })}
            required
            placeholder="ornek@email.com"
          />
        </div>
        <div className="form-group">
          <label>Sifre</label>
          <input
            type="password"
            value={form.password}
            onChange={(e) => setForm({ ...form, password: e.target.value })}
            required
            placeholder="Sifreniz"
          />
        </div>
        <button type="submit" className="btn btn-primary" disabled={loading}>
          {loading ? 'Giris yapiliyor...' : 'Giris Yap'}
        </button>
        <div className="auth-links">
          <Link to="/forgot-password">Sifremi unuttum</Link>
          <Link to="/register">Hesabin yok mu? Kayit ol</Link>
        </div>
      </form>
    </div>
  );
};

export default LoginForm;
