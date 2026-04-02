import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { register } from '../../api/authApi';
import { toast } from 'react-toastify';
import './Auth.css';

const RegisterForm = () => {
  const [form, setForm] = useState({ email: '', phone: '', password: '' });
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      await register(form);
      toast.success('Kayit basarili! Giris yapabilirsiniz.');
      navigate('/login');
    } catch (err) {
      toast.error(err.response?.data?.message || 'Kayit basarisiz');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-container">
      <form className="auth-form" onSubmit={handleSubmit}>
        <h2>Kayit Ol</h2>
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
          <label>Telefon</label>
          <input
            type="tel"
            value={form.phone}
            onChange={(e) => setForm({ ...form, phone: e.target.value })}
            required
            placeholder="05XX XXX XX XX"
          />
        </div>
        <div className="form-group">
          <label>Sifre</label>
          <input
            type="password"
            value={form.password}
            onChange={(e) => setForm({ ...form, password: e.target.value })}
            required
            minLength={6}
            placeholder="En az 6 karakter"
          />
        </div>
        <button type="submit" className="btn btn-primary" disabled={loading}>
          {loading ? 'Kayit yapiliyor...' : 'Kayit Ol'}
        </button>
        <div className="auth-links">
          <Link to="/login">Zaten hesabin var mi? Giris yap</Link>
        </div>
      </form>
    </div>
  );
};

export default RegisterForm;
