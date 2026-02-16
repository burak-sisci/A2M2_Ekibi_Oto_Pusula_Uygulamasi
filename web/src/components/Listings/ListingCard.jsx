import { Link } from 'react-router-dom';
import ShareButton from '../Share/ShareButton.jsx';
import './Listings.css';

function ListingCard({ listing }) {
    return (
        <div className="listing-card card">
            <div className="listing-card-header">
                <h3 className="listing-card-title">
                    {listing.brand} {listing.model}
                </h3>
                <span className="listing-card-year">{listing.year}</span>
            </div>
            <div className="listing-card-details">
                <div className="listing-detail-item">
                    <span className="detail-label">Kilometre</span>
                    <span className="detail-value">{listing.km?.toLocaleString('tr-TR')} km</span>
                </div>
                <div className="listing-detail-item">
                    <span className="detail-label">Yakıt</span>
                    <span className="detail-value">{listing.fuelType}</span>
                </div>
                <div className="listing-detail-item">
                    <span className="detail-label">Vites</span>
                    <span className="detail-value">{listing.gearType}</span>
                </div>
            </div>
            <div className="listing-card-price">
                {listing.price?.toLocaleString('tr-TR')} ₺
            </div>
            <div className="listing-card-actions">
                <Link to={`/listings/${listing._id}`} className="btn btn-primary">
                    Detaylar
                </Link>
                <ShareButton listingId={listing._id} title={`${listing.brand} ${listing.model}`} />
            </div>
        </div>
    );
}

export default ListingCard;
