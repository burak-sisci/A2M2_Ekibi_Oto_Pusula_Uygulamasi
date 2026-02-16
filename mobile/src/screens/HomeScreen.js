/**
 * A2M2 — Ana Sayfa (İlan Listesi)
 */

import React, { useState, useEffect } from 'react';
import { View, Text, FlatList, TouchableOpacity, StyleSheet, ActivityIndicator } from 'react-native';
import { listingsAPI } from '../services/api';
import { colors, spacing, fontSize, borderRadius } from '../styles/theme';

export default function HomeScreen({ navigation }) {
    const [listings, setListings] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        fetchListings();
    }, []);

    const fetchListings = async () => {
        try {
            const { data } = await listingsAPI.getAll();
            setListings(data);
        } catch {
            console.error('İlanlar yüklenemedi');
        } finally {
            setLoading(false);
        }
    };

    const renderItem = ({ item }) => (
        <TouchableOpacity style={styles.card} onPress={() => navigation.navigate('İlan Detayı', { id: item._id })}>
            <View style={styles.cardHeader}>
                <Text style={styles.cardTitle}>{item.brand} {item.model}</Text>
                <View style={styles.yearBadge}><Text style={styles.yearText}>{item.year}</Text></View>
            </View>
            <View style={styles.cardDetails}>
                <Text style={styles.detailText}>📏 {item.km?.toLocaleString()} km</Text>
                <Text style={styles.detailText}>⛽ {item.fuelType}</Text>
                <Text style={styles.detailText}>⚙️ {item.gearType}</Text>
            </View>
            <Text style={styles.price}>{item.price?.toLocaleString()} ₺</Text>
        </TouchableOpacity>
    );

    if (loading) return <View style={styles.center}><ActivityIndicator size="large" color={colors.primary} /></View>;

    return (
        <View style={styles.container}>
            <Text style={styles.pageTitle}>🚗 Araç İlanları</Text>
            <FlatList
                data={listings}
                keyExtractor={(item) => item._id}
                renderItem={renderItem}
                contentContainerStyle={styles.list}
                ListEmptyComponent={<Text style={styles.empty}>Henüz ilan bulunmuyor.</Text>}
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
    cardHeader: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: spacing.sm },
    cardTitle: { fontSize: fontSize.lg, fontWeight: '600', color: colors.textPrimary },
    yearBadge: { backgroundColor: colors.primary, paddingHorizontal: spacing.sm, paddingVertical: 2, borderRadius: borderRadius.sm },
    yearText: { color: '#fff', fontSize: fontSize.xs, fontWeight: '600' },
    cardDetails: { flexDirection: 'row', gap: spacing.md, marginBottom: spacing.md },
    detailText: { fontSize: fontSize.sm, color: colors.textSecondary },
    price: { fontSize: fontSize.xl, fontWeight: '700', color: colors.success },
    empty: { textAlign: 'center', color: colors.textMuted, fontSize: fontSize.md, marginTop: spacing.xl },
});
