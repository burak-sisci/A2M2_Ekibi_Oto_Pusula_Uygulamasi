import { useState } from 'react';
import { Link } from 'react-router-dom';
import { forgotPassword } from '../../api/authApi';
import { toast } from 'react-toastify';
import './Auth.css';

const ForgotPassword = () => {
  const [email, setEmail] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      await forgotPassword({ email });
      toast.success('Sifre sifirlama linki email adresinize gonderildi');
    } catch (err) {
      toast.error(err.response?.data?.message || 'Bir hata olustu');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-container">
      <form className="auth-form" onSubmit={handleSubmit}>
        <h2>Sifremi Unuttum</h2>
        <div className="form-group">
          <label>Email</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            placeholder="ornek@email.com"
          />
        </div>
        <button type="submit" className="btn btn-primary" disabled={loading}>
          {loading ? 'Gonderiliyor...' : 'Sifirlama Linki Gonder'}
        </button>
        <div className="auth-links">
          <Link to="/login">Giris sayfasina don</Link>
        </div>
      </form>
    </div>
  );
};

export default ForgotPassword;
