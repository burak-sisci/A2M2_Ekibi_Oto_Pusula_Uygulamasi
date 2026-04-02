import { useState } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { resetPassword } from '../../api/authApi';
import { toast } from 'react-toastify';
import './Auth.css';

const ResetPassword = () => {
  const [searchParams] = useSearchParams();
  const [form, setForm] = useState({
    token: searchParams.get('token') || '',
    email: searchParams.get('email') || '',
    newPassword: '',
  });
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      await resetPassword(form);
      toast.success('Sifreniz basariyla degistirildi');
      navigate('/login');
    } catch (err) {
      toast.error(err.response?.data?.message || 'Bir hata olustu');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-container">
      <form className="auth-form" onSubmit={handleSubmit}>
        <h2>Sifre Sifirla</h2>
        <div className="form-group">
          <label>Email</label>
          <input
            type="email"
            value={form.email}
            onChange={(e) => setForm({ ...form, email: e.target.value })}
            required
          />
        </div>
        <div className="form-group">
          <label>Token</label>
          <input
            type="text"
            value={form.token}
            onChange={(e) => setForm({ ...form, token: e.target.value })}
            required
          />
        </div>
        <div className="form-group">
          <label>Yeni Sifre</label>
          <input
            type="password"
            value={form.newPassword}
            onChange={(e) => setForm({ ...form, newPassword: e.target.value })}
            required
            minLength={6}
            placeholder="En az 6 karakter"
          />
        </div>
        <button type="submit" className="btn btn-primary" disabled={loading}>
          {loading ? 'Sifirlaniyor...' : 'Sifreyi Sifirla'}
        </button>
      </form>
    </div>
  );
};

export default ResetPassword;
