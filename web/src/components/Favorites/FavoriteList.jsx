import { useState, useEffect } from 'react';
import { favoritesAPI } from '../../services/api.js';
import ListingCard from '../Listings/ListingCard.jsx';
import './Favorites.css';

function FavoriteList() {
    const [favorites, setFavorites] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        fetchFavorites();
    }, []);

    const fetchFavorites = async () => {
        try {
            const { data } = await favoritesAPI.getAll();
            setFavorites(data);
        } catch {
            console.error('Favoriler yüklenirken hata oluştu');
        } finally {
            setLoading(false);
        }
    };

    const handleRemove = async (listingId) => {
        try {
            await favoritesAPI.remove(listingId);
            setFavorites(favorites.filter((f) => f.listingId?._id !== listingId));
        } catch {
            console.error('Favori silinirken hata oluştu');
        }
    };

    if (loading) return <div className="loading-spinner">Yükleniyor...</div>;

    return (
        <div className="favorites-page">
            <h1 className="page-title">❤️ Favorilerim</h1>
            {favorites.length === 0 ? (
                <div className="empty-state">
                    <p>Henüz favorilere eklenen ilan yok.</p>
                </div>
            ) : (
                <div className="listings-grid">
                    {favorites.map((fav) => (
                        <div key={fav._id} className="favorite-item">
                            <ListingCard listing={fav.listingId} />
                            <button className="btn btn-danger favorite-remove-btn" onClick={() => handleRemove(fav.listingId?._id)}>
                                Favorilerden Kaldır
                            </button>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
}

export default FavoriteList;
