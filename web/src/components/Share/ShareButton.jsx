import { shareAPI } from '../../services/api.js';

function ShareButton({ listingId, title }) {
    const handleShare = async () => {
        try {
            const { data } = await shareAPI.getShareInfo(listingId);

            // Web Share API desteği varsa kullan
            if (navigator.share) {
                await navigator.share({
                    title: data.title,
                    text: data.description,
                    url: data.url,
                });
            } else {
                // Yoksa clipboard'a kopyala
                await navigator.clipboard.writeText(data.url);
                alert('Link panoya kopyalandı!');
            }
        } catch (err) {
            if (err.name !== 'AbortError') {
                console.error('Paylaşım hatası:', err);
            }
        }
    };

    return (
        <button className="btn btn-secondary" onClick={handleShare} title="Paylaş">
            🔗 Paylaş
        </button>
    );
}

export default ShareButton;
