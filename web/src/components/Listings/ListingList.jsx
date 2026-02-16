import { useState, useEffect } from 'react';
import { listingsAPI } from '../../services/api.js';
import ListingCard from './ListingCard.jsx';
import './Listings.css';

function ListingList() {
    const [listings, setListings] = useState([]);
    const [loading, setLoading] = useState(true);
    const [filters, setFilters] = useState({ brand: '', fuelType: '', sort: '' });

    useEffect(() => {
        fetchListings();
    }, [filters]);

    const fetchListings = async () => {
        try {
            setLoading(true);
            const params = {};
            if (filters.brand) params.brand = filters.brand;
            if (filters.fuelType) params.fuelType = filters.fuelType;
            if (filters.sort) params.sort = filters.sort;
            const { data } = await listingsAPI.getAll(params);
            setListings(data);
        } catch {
            console.error('İlanlar yüklenirken hata oluştu');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="listings-page">
            <h1 className="page-title">🚗 Araç İlanları</h1>

            <div className="listings-filters">
                <input type="text" className="form-control" placeholder="Marka ara..."
                    value={filters.brand} onChange={(e) => setFilters({ ...filters, brand: e.target.value })} />
                <select className="form-control"
                    value={filters.fuelType} onChange={(e) => setFilters({ ...filters, fuelType: e.target.value })}>
                    <option value="">Tüm Yakıt Tipleri</option>
                    <option value="Benzin">Benzin</option>
                    <option value="Dizel">Dizel</option>
                    <option value="LPG">LPG</option>
                    <option value="Elektrik">Elektrik</option>
                    <option value="Hibrit">Hibrit</option>
                </select>
                <select className="form-control"
                    value={filters.sort} onChange={(e) => setFilters({ ...filters, sort: e.target.value })}>
                    <option value="">En Yeni</option>
                    <option value="price_asc">Fiyat: Düşükten Yükseğe</option>
                    <option value="price_desc">Fiyat: Yüksekten Düşüğe</option>
                </select>
            </div>

            {loading ? (
                <div className="loading-spinner">Yükleniyor...</div>
            ) : listings.length === 0 ? (
                <div className="empty-state">
                    <p>Henüz ilan bulunmuyor.</p>
                </div>
            ) : (
                <div className="listings-grid">
                    {listings.map((listing) => (
                        <ListingCard key={listing._id} listing={listing} />
                    ))}
                </div>
            )}
        </div>
    );
}

export default ListingList;
