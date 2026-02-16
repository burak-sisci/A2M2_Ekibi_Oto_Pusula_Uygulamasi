/**
 * A2M2 — Kayıt Ekranı
 */

import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, StyleSheet, Alert } from 'react-native';
import { useAuth } from '../context/AuthContext';
import { authAPI } from '../services/api';
import { colors, spacing, fontSize, borderRadius } from '../styles/theme';

export default function RegisterScreen({ navigation }) {
    const [name, setName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const { login } = useAuth();

    const handleRegister = async () => {
        if (!name || !email || !password) return Alert.alert('Hata', 'Tüm alanları doldurun');
        setLoading(true);
        try {
            const { data } = await authAPI.register({ name, email, password });
            await login(data);
        } catch (err) {
            Alert.alert('Hata', err.response?.data?.message || 'Kayıt olunamadı');
        } finally {
            setLoading(false);
        }
    };

    return (
        <View style={styles.container}>
            <Text style={styles.title}>Kayıt Ol</Text>
            <Text style={styles.subtitle}>A2M2'ye ücretsiz üye olun</Text>

            <TextInput style={styles.input} placeholder="Ad Soyad" placeholderTextColor={colors.textMuted} value={name} onChangeText={setName} />
            <TextInput style={styles.input} placeholder="E-posta" placeholderTextColor={colors.textMuted} value={email} onChangeText={setEmail} keyboardType="email-address" autoCapitalize="none" />
            <TextInput style={styles.input} placeholder="Şifre (en az 6 karakter)" placeholderTextColor={colors.textMuted} value={password} onChangeText={setPassword} secureTextEntry />

            <TouchableOpacity style={styles.button} onPress={handleRegister} disabled={loading}>
                <Text style={styles.buttonText}>{loading ? 'Kayıt yapılıyor...' : 'Kayıt Ol'}</Text>
            </TouchableOpacity>

            <TouchableOpacity onPress={() => navigation.navigate('Giriş')}>
                <Text style={styles.link}>Zaten hesabınız var mı? <Text style={styles.linkHighlight}>Giriş Yap</Text></Text>
            </TouchableOpacity>
        </View>
    );
}

const styles = StyleSheet.create({
    container: { flex: 1, backgroundColor: colors.bgPrimary, justifyContent: 'center', padding: spacing.xl },
    title: { fontSize: fontSize.xxl, fontWeight: '700', color: colors.primaryLight, marginBottom: spacing.xs },
    subtitle: { fontSize: fontSize.sm, color: colors.textSecondary, marginBottom: spacing.xl },
    input: { backgroundColor: colors.bgInput, color: colors.textPrimary, padding: spacing.md, borderRadius: borderRadius.md, marginBottom: spacing.md, fontSize: fontSize.md, borderWidth: 1, borderColor: colors.borderColor },
    button: { backgroundColor: colors.primary, padding: spacing.md, borderRadius: borderRadius.md, alignItems: 'center', marginTop: spacing.sm },
    buttonText: { color: '#fff', fontSize: fontSize.md, fontWeight: '600' },
    link: { textAlign: 'center', marginTop: spacing.lg, color: colors.textSecondary, fontSize: fontSize.sm },
    linkHighlight: { color: colors.primaryLight },
});
