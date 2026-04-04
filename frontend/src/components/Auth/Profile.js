import { useState } from 'react';
import { useAuth } from '../../context/AuthContext';
import { updateProfile, deleteAccount } from '../../api/authApi';
import { toast } from 'react-toastify';
import { useNavigate } from 'react-router-dom';
import './Auth.css';

const Profile = () => {
  const { user, logoutUser } = useAuth();
  const [phone, setPhone] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleUpdate = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      await updateProfile({ phone });
      toast.success('Profil guncellendi');
    } catch (err) {
      toast.error(err.response?.data?.message || 'Guncelleme basarisiz');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (!window.confirm('Hesabinizi silmek istediginize emin misiniz? Bu islem geri alinamaz.')) return;
    try {
      await deleteAccount(user.id);
      logoutUser();
      toast.success('Hesabiniz silindi');
      navigate('/');
    } catch (err) {
      toast.error(err.response?.data?.message || 'Silme basarisiz');
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-form">
        <h2>Profilim</h2>
        <div className="profile-info">
          <p><strong>Email:</strong> {user?.email}</p>
        </div>
        <form onSubmit={handleUpdate}>
          <div className="form-group">
            <label>Telefon Numarasi</label>
            <input
              type="tel"
              value={phone}
              onChange={(e) => setPhone(e.target.value)}
              placeholder="05XX XXX XX XX"
            />
          </div>
          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? 'Guncelleniyor...' : 'Telefonu Guncelle'}
          </button>
        </form>
        <hr style={{ margin: '24px 0', borderColor: '#333' }} />
        <button onClick={handleDelete} className="btn btn-danger">
          Hesabi Sil
        </button>
      </div>
    </div>
  );
};

export default Profile;
