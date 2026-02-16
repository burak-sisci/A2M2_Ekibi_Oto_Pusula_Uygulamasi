/**
 * A2M2 — İlan Detay Ekranı
 */

import React, { useState, useEffect } from 'react';
import { View, Text, ScrollView, TouchableOpacity, StyleSheet, Alert, Share, ActivityIndicator } from 'react-native';
import { listingsAPI, favoritesAPI, shareAPI } from '../services/api';
import { useAuth } from '../context/AuthContext';
import { colors, spacing, fontSize, borderRadius } from '../styles/theme';

export default function ListingDetailScreen({ route }) {
    const { id } = route.params;
    const { isAuthenticated } = useAuth();
    const [listing, setListing] = useState(null);
    const [loading, setLoading] = useState(true);
    const [isFavorite, setIsFavorite] = useState(false);

    useEffect(() => { fetchListing(); }, [id]);

    const fetchListing = async () => {
        try {
            const { data } = await listingsAPI.getById(id);
            setListing(data);
        } catch { Alert.alert('Hata', 'İlan yüklenemedi'); }
        finally { setLoading(false); }
    };

    const handleFavorite = async () => {
        try {
            if (isFavorite) {
                await favoritesAPI.remove(id);
                setIsFavorite(false);
            } else {
                await favoritesAPI.add(id);
                setIsFavorite(true);
            }
        } catch { Alert.alert('Hata', 'Favori işlemi başarısız'); }
    };

    const handleShare = async () => {
        try {
            const { data } = await shareAPI.getShareInfo(id);
            await Share.share({ title: data.title, message: `${data.description}\n${data.url}` });
        } catch { /* cancelled */ }
    };

    if (loading) return <View style={styles.center}><ActivityIndicator size="large" color={colors.primary} /></View>;
    if (!listing) return <View style={styles.center}><Text style={styles.empty}>İlan bulunamadı</Text></View>;

    return (
        <ScrollView style={styles.container}>
            <Text style={styles.title}>{listing.brand} {listing.model}</Text>
            <Text style={styles.price}>{listing.price?.toLocaleString()} ₺</Text>

            <View style={styles.detailsCard}>
                <DetailRow label="Yıl" value={listing.year} />
                <DetailRow label="Kilometre" value={`${listing.km?.toLocaleString()} km`} />
                <DetailRow label="Yakıt" value={listing.fuelType} />
                <DetailRow label="Vites" value={listing.gearType} />
                {listing.description ? <DetailRow label="Açıklama" value={listing.description} /> : null}
                <DetailRow label="İlan Sahibi" value={listing.userId?.name || '-'} />
            </View>

            <View style={styles.actions}>
                {isAuthenticated && (
                    <TouchableOpacity style={[styles.btn, isFavorite ? styles.btnDanger : styles.btnSecondary]} onPress={handleFavorite}>
                        <Text style={styles.btnText}>{isFavorite ? '❤️ Favorilerden Çıkar' : '🤍 Favorilere Ekle'}</Text>
                    </TouchableOpacity>
                )}
                <TouchableOpacity style={[styles.btn, styles.btnSecondary]} onPress={handleShare}>
                    <Text style={styles.btnText}>🔗 Paylaş</Text>
                </TouchableOpacity>
            </View>
        </ScrollView>
    );
}

function DetailRow({ label, value }) {
    return (
        <View style={styles.detailRow}>
            <Text style={styles.detailLabel}>{label}</Text>
            <Text style={styles.detailValue}>{value}</Text>
        </View>
    );
}

const styles = StyleSheet.create({
    container: { flex: 1, backgroundColor: colors.bgPrimary, padding: spacing.lg },
    center: { flex: 1, justifyContent: 'center', alignItems: 'center', backgroundColor: colors.bgPrimary },
    empty: { color: colors.textMuted },
    title: { fontSize: fontSize.xl, fontWeight: '700', color: colors.primaryLight, marginBottom: spacing.sm },
    price: { fontSize: 28, fontWeight: '800', color: colors.success, marginBottom: spacing.lg },
    detailsCard: { backgroundColor: colors.bgCard, borderRadius: borderRadius.lg, padding: spacing.lg, borderWidth: 1, borderColor: colors.borderColor, marginBottom: spacing.lg },
    detailRow: { flexDirection: 'row', justifyContent: 'space-between', paddingVertical: spacing.sm, borderBottomWidth: 1, borderBottomColor: colors.borderColor },
    detailLabel: { fontSize: fontSize.sm, color: colors.textMuted },
    detailValue: { fontSize: fontSize.sm, color: colors.textPrimary, fontWeight: '500' },
    actions: { gap: spacing.sm },
    btn: { padding: spacing.md, borderRadius: borderRadius.md, alignItems: 'center' },
    btnSecondary: { backgroundColor: colors.bgInput, borderWidth: 1, borderColor: colors.borderColor },
    btnDanger: { backgroundColor: colors.danger },
    btnText: { color: colors.textPrimary, fontSize: fontSize.md, fontWeight: '500' },
});
