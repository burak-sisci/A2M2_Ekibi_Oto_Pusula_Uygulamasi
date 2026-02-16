/**
 * A2M2 — Favoriler Ekranı
 */

import React, { useState, useEffect } from 'react';
import { View, Text, FlatList, TouchableOpacity, StyleSheet, Alert, ActivityIndicator } from 'react-native';
import { favoritesAPI } from '../services/api';
import { colors, spacing, fontSize, borderRadius } from '../styles/theme';

export default function FavoritesScreen({ navigation }) {
    const [favorites, setFavorites] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => { fetchFavorites(); }, []);

    const fetchFavorites = async () => {
        try {
            const { data } = await favoritesAPI.getAll();
            setFavorites(data);
        } catch { console.error('Favoriler yüklenemedi'); }
        finally { setLoading(false); }
    };

    const handleRemove = async (listingId) => {
        try {
            await favoritesAPI.remove(listingId);
            setFavorites(favorites.filter((f) => f.listingId?._id !== listingId));
        } catch { Alert.alert('Hata', 'Favori silinemedi'); }
    };

    const renderItem = ({ item }) => {
        const listing = item.listingId;
        if (!listing) return null;
        return (
            <View style={styles.card}>
                <TouchableOpacity onPress={() => navigation.navigate('İlan Detayı', { id: listing._id })}>
                    <Text style={styles.cardTitle}>{listing.brand} {listing.model} ({listing.year})</Text>
                    <Text style={styles.price}>{listing.price?.toLocaleString()} ₺</Text>
                </TouchableOpacity>
                <TouchableOpacity style={styles.removeBtn} onPress={() => handleRemove(listing._id)}>
                    <Text style={styles.removeBtnText}>Favorilerden Kaldır</Text>
                </TouchableOpacity>
            </View>
        );
    };

    if (loading) return <View style={styles.center}><ActivityIndicator size="large" color={colors.primary} /></View>;

    return (
        <View style={styles.container}>
            <Text style={styles.pageTitle}>❤️ Favorilerim</Text>
            <FlatList
                data={favorites}
                keyExtractor={(item) => item._id}
                renderItem={renderItem}
                contentContainerStyle={styles.list}
                ListEmptyComponent={<Text style={styles.empty}>Henüz favori ilanınız yok.</Text>}
            />
        </View>
    );
}

const styles = StyleSheet.create({
    container: { flex: 1, backgroundColor: colors.bgPrimary },
    center: { flex: 1, justifyContent: 'center', alignItems: 'center', backgroundColor: colors.bgPrimary },
    pageTitle: { fontSize: fontSize.xl, fontWeight: '700', color: colors.primaryLight, padding: spacing.lg, paddingBottom: spacing.sm },
    list: { padding: spacing.md, gap: spacing.md },
    card: { backgroundColor: colors.bgCard, borderRadius: borderRadius.lg, padding: spacing.lg, borderWidth: 1, borderColor: colors.borderColor },
    cardTitle: { fontSize: fontSize.lg, fontWeight: '600', color: colors.textPrimary, marginBottom: spacing.xs },
    price: { fontSize: fontSize.lg, fontWeight: '700', color: colors.success, marginBottom: spacing.md },
    removeBtn: { backgroundColor: colors.danger, padding: spacing.sm, borderRadius: borderRadius.md, alignItems: 'center' },
    removeBtnText: { color: '#fff', fontSize: fontSize.sm, fontWeight: '500' },
    empty: { textAlign: 'center', color: colors.textMuted, fontSize: fontSize.md, marginTop: spacing.xl },
});
