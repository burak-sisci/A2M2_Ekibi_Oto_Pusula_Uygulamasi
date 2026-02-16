import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { listingsAPI } from '../../services/api.js';
import './Listings.css';

function CreateListing() {
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        brand: '', model: '', year: '', km: '', fuelType: 'Benzin', gearType: 'Manuel', price: '', description: '',
    });
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');
        try {
            await listingsAPI.create({
                ...formData,
                year: Number(formData.year),
                km: Number(formData.km),
                price: Number(formData.price),
            });
            navigate('/');
        } catch (err) {
            setError(err.response?.data?.message || 'İlan oluşturulurken bir hata oluştu');
        } finally {
            setLoading(false);
        }
    };

    const handleChange = (e) => setFormData({ ...formData, [e.target.name]: e.target.value });

    return (
        <div className="create-listing">
            <h1 className="page-title">📝 Yeni İlan Oluştur</h1>
            <div className="card create-listing-form">
                {error && <div className="auth-error">{error}</div>}
                <form onSubmit={handleSubmit}>
                    <div className="form-row">
                        <div className="form-group">
                            <label>Marka</label>
                            <input name="brand" className="form-control" placeholder="ör: Toyota" value={formData.brand} onChange={handleChange} required />
                        </div>
                        <div className="form-group">
                            <label>Model</label>
                            <input name="model" className="form-control" placeholder="ör: Corolla" value={formData.model} onChange={handleChange} required />
                        </div>
                    </div>
                    <div className="form-row">
                        <div className="form-group">
                            <label>Yıl</label>
                            <input name="year" type="number" className="form-control" placeholder="ör: 2020" value={formData.year} onChange={handleChange} required />
                        </div>
                        <div className="form-group">
                            <label>Kilometre</label>
                            <input name="km" type="number" className="form-control" placeholder="ör: 50000" value={formData.km} onChange={handleChange} required />
                        </div>
                    </div>
                    <div className="form-row">
                        <div className="form-group">
                            <label>Yakıt Tipi</label>
                            <select name="fuelType" className="form-control" value={formData.fuelType} onChange={handleChange}>
                                <option>Benzin</option><option>Dizel</option><option>LPG</option><option>Elektrik</option><option>Hibrit</option>
                            </select>
                        </div>
                        <div className="form-group">
                            <label>Vites Tipi</label>
                            <select name="gearType" className="form-control" value={formData.gearType} onChange={handleChange}>
                                <option>Manuel</option><option>Otomatik</option><option>Yarı Otomatik</option>
                            </select>
                        </div>
                    </div>
                    <div className="form-group">
                        <label>Fiyat (₺)</label>
                        <input name="price" type="number" className="form-control" placeholder="ör: 850000" value={formData.price} onChange={handleChange} required />
                    </div>
                    <div className="form-group">
                        <label>Açıklama</label>
                        <textarea name="description" className="form-control" rows="3" placeholder="Araç hakkında ek bilgiler..." value={formData.description} onChange={handleChange} />
                    </div>
                    <button type="submit" className="btn btn-primary auth-btn" disabled={loading}>
                        {loading ? 'Oluşturuluyor...' : 'İlan Oluştur'}
                    </button>
                </form>
            </div>
        </div>
    );
}

export default CreateListing;
