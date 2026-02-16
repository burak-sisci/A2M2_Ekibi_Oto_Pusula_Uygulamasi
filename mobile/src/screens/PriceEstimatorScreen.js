/**
 * A2M2 — Fiyat Tahmin Ekranı
 */

import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, ScrollView, StyleSheet, Alert } from 'react-native';
import { aiAPI } from '../services/api';
import { colors, spacing, fontSize, borderRadius } from '../styles/theme';

export default function PriceEstimatorScreen() {
    const [form, setForm] = useState({
        brand: '', model: '', year: '', km: '', fuelType: 'Benzin', gearType: 'Manuel',
    });
    const [result, setResult] = useState(null);
    const [loading, setLoading] = useState(false);

    const handlePredict = async () => {
        if (!form.brand || !form.model || !form.year || !form.km) return Alert.alert('Hata', 'Tüm alanları doldurun');
        setLoading(true);
        setResult(null);
        try {
            const { data } = await aiAPI.predictPrice({ ...form, year: Number(form.year), km: Number(form.km) });
            setResult(data);
        } catch {
            Alert.alert('Hata', 'Fiyat tahmini yapılamadı');
        } finally {
            setLoading(false);
        }
    };

    const update = (key, val) => setForm({ ...form, [key]: val });

    return (
        <ScrollView style={styles.container}>
            <Text style={styles.title}>🤖 Fiyat Tahmini</Text>
            <Text style={styles.subtitle}>Aracınızın özelliklerini girerek piyasa değerini öğrenin</Text>

            <TextInput style={styles.input} placeholder="Marka" placeholderTextColor={colors.textMuted} value={form.brand} onChangeText={(v) => update('brand', v)} />
            <TextInput style={styles.input} placeholder="Model" placeholderTextColor={colors.textMuted} value={form.model} onChangeText={(v) => update('model', v)} />
            <TextInput style={styles.input} placeholder="Yıl" placeholderTextColor={colors.textMuted} value={form.year} onChangeText={(v) => update('year', v)} keyboardType="numeric" />
            <TextInput style={styles.input} placeholder="Kilometre" placeholderTextColor={colors.textMuted} value={form.km} onChangeText={(v) => update('km', v)} keyboardType="numeric" />

            <TouchableOpacity style={styles.button} onPress={handlePredict} disabled={loading}>
                <Text style={styles.buttonText}>{loading ? 'Hesaplanıyor...' : '💰 Fiyat Hesapla'}</Text>
            </TouchableOpacity>

            {result && (
                <View style={styles.resultCard}>
                    <Text style={styles.resultLabel}>Tahmini Piyasa Değeri</Text>
                    <Text style={styles.resultPrice}>{result.predictedPrice?.toLocaleString()} ₺</Text>
                    <Text style={styles.resultNote}>Yapay zeka tarafından güncel verilerle hesaplanmıştır.</Text>
                </View>
            )}
        </ScrollView>
    );
}

const styles = StyleSheet.create({
    container: { flex: 1, backgroundColor: colors.bgPrimary, padding: spacing.lg },
    title: { fontSize: fontSize.xl, fontWeight: '700', color: colors.primaryLight, marginBottom: spacing.xs },
    subtitle: { fontSize: fontSize.sm, color: colors.textSecondary, marginBottom: spacing.xl },
    input: { backgroundColor: colors.bgInput, color: colors.textPrimary, padding: spacing.md, borderRadius: borderRadius.md, marginBottom: spacing.md, fontSize: fontSize.md, borderWidth: 1, borderColor: colors.borderColor },
    button: { backgroundColor: colors.primary, padding: spacing.md, borderRadius: borderRadius.md, alignItems: 'center', marginTop: spacing.sm },
    buttonText: { color: '#fff', fontSize: fontSize.md, fontWeight: '600' },
    resultCard: { backgroundColor: colors.bgCard, borderRadius: borderRadius.lg, padding: spacing.xl, marginTop: spacing.xl, alignItems: 'center', borderWidth: 1, borderColor: colors.success },
    resultLabel: { fontSize: fontSize.sm, color: colors.textSecondary, marginBottom: spacing.sm },
    resultPrice: { fontSize: 32, fontWeight: '800', color: colors.success },
    resultNote: { fontSize: fontSize.xs, color: colors.textMuted, marginTop: spacing.md, textAlign: 'center' },
});
