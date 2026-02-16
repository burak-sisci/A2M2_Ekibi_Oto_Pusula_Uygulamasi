/**
 * A2M2 — İlan Oluşturma Ekranı
 */

import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, ScrollView, StyleSheet, Alert } from 'react-native';
import { listingsAPI } from '../services/api';
import { colors, spacing, fontSize, borderRadius } from '../styles/theme';

export default function CreateListingScreen({ navigation }) {
    const [form, setForm] = useState({
        brand: '', model: '', year: '', km: '', fuelType: 'Benzin', gearType: 'Manuel', price: '', description: '',
    });
    const [loading, setLoading] = useState(false);

    const handleCreate = async () => {
        if (!form.brand || !form.model || !form.year || !form.km || !form.price) {
            return Alert.alert('Hata', 'Zorunlu alanları doldurun');
        }
        setLoading(true);
        try {
            await listingsAPI.create({
                ...form, year: Number(form.year), km: Number(form.km), price: Number(form.price),
            });
            Alert.alert('Başarılı', 'İlan oluşturuldu');
            navigation.goBack();
        } catch (err) {
            Alert.alert('Hata', err.response?.data?.message || 'İlan oluşturulamadı');
        } finally {
            setLoading(false);
        }
    };

    const update = (key, val) => setForm({ ...form, [key]: val });

    return (
        <ScrollView style={styles.container}>
            <Text style={styles.title}>📝 Yeni İlan</Text>
            <TextInput style={styles.input} placeholder="Marka" placeholderTextColor={colors.textMuted} value={form.brand} onChangeText={(v) => update('brand', v)} />
            <TextInput style={styles.input} placeholder="Model" placeholderTextColor={colors.textMuted} value={form.model} onChangeText={(v) => update('model', v)} />
            <TextInput style={styles.input} placeholder="Yıl" placeholderTextColor={colors.textMuted} value={form.year} onChangeText={(v) => update('year', v)} keyboardType="numeric" />
            <TextInput style={styles.input} placeholder="Kilometre" placeholderTextColor={colors.textMuted} value={form.km} onChangeText={(v) => update('km', v)} keyboardType="numeric" />
            <TextInput style={styles.input} placeholder="Fiyat (₺)" placeholderTextColor={colors.textMuted} value={form.price} onChangeText={(v) => update('price', v)} keyboardType="numeric" />
            <TextInput style={[styles.input, { height: 80 }]} placeholder="Açıklama" placeholderTextColor={colors.textMuted} value={form.description} onChangeText={(v) => update('description', v)} multiline />

            <TouchableOpacity style={styles.button} onPress={handleCreate} disabled={loading}>
                <Text style={styles.buttonText}>{loading ? 'Oluşturuluyor...' : 'İlan Oluştur'}</Text>
            </TouchableOpacity>
        </ScrollView>
    );
}

const styles = StyleSheet.create({
    container: { flex: 1, backgroundColor: colors.bgPrimary, padding: spacing.lg },
    title: { fontSize: fontSize.xl, fontWeight: '700', color: colors.primaryLight, marginBottom: spacing.lg },
    input: { backgroundColor: colors.bgInput, color: colors.textPrimary, padding: spacing.md, borderRadius: borderRadius.md, marginBottom: spacing.md, fontSize: fontSize.md, borderWidth: 1, borderColor: colors.borderColor },
    button: { backgroundColor: colors.primary, padding: spacing.md, borderRadius: borderRadius.md, alignItems: 'center', marginTop: spacing.sm, marginBottom: spacing.xl },
    buttonText: { color: '#fff', fontSize: fontSize.md, fontWeight: '600' },
});
