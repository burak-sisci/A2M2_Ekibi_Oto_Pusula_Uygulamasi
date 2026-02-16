/**
 * A2M2 — Mobil Uygulama Giriş Noktası
 */

import React from 'react';
import { StatusBar } from 'react-native';
import { AuthProvider } from './src/context/AuthContext';
import AppNavigator from './src/navigation/AppNavigator';

export default function App() {
    return (
        <AuthProvider>
            <StatusBar barStyle="light-content" backgroundColor="#0f172a" />
            <AppNavigator />
        </AuthProvider>
    );
}
