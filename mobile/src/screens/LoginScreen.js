/**
 * A2M2 — Giriş Ekranı
 * MehmetO tarafından geliştirilecek ekranın iskeletidir.
 */

import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, StyleSheet, Alert } from 'react-native';
import { useAuth } from '../context/AuthContext';
import { authAPI } from '../services/api';
import { colors, spacing, fontSize, borderRadius } from '../styles/theme';

export default function LoginScreen({ navigation }) {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const { login } = useAuth();

    const handleLogin = async () => {
        if (!email || !password) return Alert.alert('Hata', 'Tüm alanları doldurun');
        setLoading(true);
        try {
            const { data } = await authAPI.login({ email, password });
            await login(data);
        } catch (err) {
            Alert.alert('Hata', err.response?.data?.message || 'Giriş yapılamadı');
        } finally {
            setLoading(false);
        }
    };

    return (
        <View style={styles.container}>
            <Text style={styles.title}>Giriş Yap</Text>
            <Text style={styles.subtitle}>A2M2 hesabınıza giriş yapın</Text>

            <TextInput style={styles.input} placeholder="E-posta" placeholderTextColor={colors.textMuted}
                value={email} onChangeText={setEmail} keyboardType="email-address" autoCapitalize="none" />
            <TextInput style={styles.input} placeholder="Şifre" placeholderTextColor={colors.textMuted}
                value={password} onChangeText={setPassword} secureTextEntry />

            <TouchableOpacity style={styles.button} onPress={handleLogin} disabled={loading}>
                <Text style={styles.buttonText}>{loading ? 'Giriş yapılıyor...' : 'Giriş Yap'}</Text>
            </TouchableOpacity>

            <TouchableOpacity onPress={() => navigation.navigate('Kayıt')}>
                <Text style={styles.link}>Hesabınız yok mu? <Text style={styles.linkHighlight}>Kayıt Ol</Text></Text>
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
