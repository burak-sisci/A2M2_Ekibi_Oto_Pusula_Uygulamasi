import { useState } from 'react';
import { aiAPI } from '../../services/api.js';
import './PriceEstimator.css';

function PriceEstimator() {
    const [formData, setFormData] = useState({
        brand: '', model: '', year: '', km: '', fuelType: 'Benzin', gearType: 'Manuel',
    });
    const [result, setResult] = useState(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');
        setResult(null);
        try {
            const { data } = await aiAPI.predictPrice({
                ...formData,
                year: Number(formData.year),
                km: Number(formData.km),
            });
            setResult(data);
        } catch {
            setError('Fiyat tahmini yapılırken bir hata oluştu');
        } finally {
            setLoading(false);
        }
    };

    const handleChange = (e) => setFormData({ ...formData, [e.target.name]: e.target.value });

    return (
        <div className="price-estimator">
            <h1 className="page-title">🤖 Araç Fiyat Tahmini</h1>
            <p className="estimator-description">
                Aracınızın özelliklerini girerek yapay zeka destekli güncel piyasa değerini öğrenin.
            </p>

            <div className="estimator-container">
                <div className="card estimator-form-card">
                    <h2>Araç Bilgileri</h2>
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
                        <button type="submit" className="btn btn-primary auth-btn" disabled={loading}>
                            {loading ? 'Hesaplanıyor...' : '💰 Fiyat Hesapla'}
                        </button>
                    </form>
                </div>

                {result && (
                    <div className="card estimator-result-card">
                        <h2>Tahmini Piyasa Değeri</h2>
                        <div className="predicted-price">
                            {result.predictedPrice?.toLocaleString('tr-TR')} ₺
                        </div>
                        <p className="result-note">
                            Bu tahmin, yapay zeka modelimiz tarafından güncel piyasa verilerine göre hesaplanmıştır.
                        </p>
                    </div>
                )}
            </div>
        </div>
    );
}

export default PriceEstimator;
