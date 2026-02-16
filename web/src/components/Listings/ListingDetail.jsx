import { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { listingsAPI, favoritesAPI } from '../../services/api.js';
import { useAuth } from '../../context/AuthContext.jsx';
import ShareButton from '../Share/ShareButton.jsx';
import './Listings.css';

function ListingDetail() {
    const { id } = useParams();
    const { isAuthenticated } = useAuth();
    const [listing, setListing] = useState(null);
    const [loading, setLoading] = useState(true);
    const [isFavorite, setIsFavorite] = useState(false);

    useEffect(() => {
        fetchListing();
    }, [id]);

    const fetchListing = async () => {
        try {
            const { data } = await listingsAPI.getById(id);
            setListing(data);
        } catch {
            console.error('İlan yüklenirken hata oluştu');
        } finally {
            setLoading(false);
        }
    };

    const handleFavoriteToggle = async () => {
        try {
            if (isFavorite) {
                await favoritesAPI.remove(id);
                setIsFavorite(false);
            } else {
                await favoritesAPI.add(id);
                setIsFavorite(true);
            }
        } catch {
            console.error('Favori işlemi sırasında hata oluştu');
        }
    };

    if (loading) return <div className="loading-spinner">Yükleniyor...</div>;
    if (!listing) return <div className="empty-state">İlan bulunamadı.</div>;

    return (
        <div className="listing-detail">
            <div className="listing-detail-header">
                <h1 className="page-title">{listing.brand} {listing.model}</h1>
                <div className="listing-detail-actions">
                    {isAuthenticated && (
                        <button className={`btn ${isFavorite ? 'btn-danger' : 'btn-secondary'}`} onClick={handleFavoriteToggle}>
                            {isFavorite ? '❤️ Favorilerden Çıkar' : '🤍 Favorilere Ekle'}
                        </button>
                    )}
                    <ShareButton listingId={listing._id} title={`${listing.brand} ${listing.model}`} />
                </div>
            </div>

            <div className="listing-detail-card card">
                <div className="detail-grid">
                    <div className="detail-row"><span className="detail-label">Yıl</span><span className="detail-value">{listing.year}</span></div>
                    <div className="detail-row"><span className="detail-label">Kilometre</span><span className="detail-value">{listing.km?.toLocaleString('tr-TR')} km</span></div>
                    <div className="detail-row"><span className="detail-label">Yakıt Tipi</span><span className="detail-value">{listing.fuelType}</span></div>
                    <div className="detail-row"><span className="detail-label">Vites Tipi</span><span className="detail-value">{listing.gearType}</span></div>
                    <div className="detail-row"><span className="detail-label">Fiyat</span><span className="detail-value price-highlight">{listing.price?.toLocaleString('tr-TR')} ₺</span></div>
                </div>
                {listing.description && (
                    <div className="listing-description">
                        <h3>Açıklama</h3>
                        <p>{listing.description}</p>
                    </div>
                )}
                <div className="listing-seller">
                    <h3>İlan Sahibi</h3>
                    <p>{listing.userId?.name}</p>
                </div>
            </div>
        </div>
    );
}

export default ListingDetail;
