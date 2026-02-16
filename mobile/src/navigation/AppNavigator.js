/**
 * A2M2 — Mobil Navigasyon
 * Bottom tab + stack navigator yapısı
 */

import React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { Text } from 'react-native';
import { useAuth } from '../context/AuthContext';
import { colors } from '../styles/theme';

// Screens
import LoginScreen from '../screens/LoginScreen';
import RegisterScreen from '../screens/RegisterScreen';
import HomeScreen from '../screens/HomeScreen';
import ListingDetailScreen from '../screens/ListingDetailScreen';
import CreateListingScreen from '../screens/CreateListingScreen';
import FavoritesScreen from '../screens/FavoritesScreen';
import PriceEstimatorScreen from '../screens/PriceEstimatorScreen';

const Stack = createNativeStackNavigator();
const Tab = createBottomTabNavigator();

const screenOptions = {
    headerStyle: { backgroundColor: colors.bgSecondary },
    headerTintColor: colors.textPrimary,
    headerTitleStyle: { fontWeight: '600' },
};

function HomeTabs() {
    const { isAuthenticated } = useAuth();

    return (
        <Tab.Navigator
            screenOptions={{
                tabBarStyle: { backgroundColor: colors.bgSecondary, borderTopColor: colors.borderColor },
                tabBarActiveTintColor: colors.primary,
                tabBarInactiveTintColor: colors.textMuted,
                headerShown: false,
            }}
        >
            <Tab.Screen name="İlanlar" component={HomeScreen}
                options={{ tabBarIcon: ({ color }) => <Text style={{ color, fontSize: 20 }}>🚗</Text> }} />
            <Tab.Screen name="Fiyat Tahmini" component={PriceEstimatorScreen}
                options={{ tabBarIcon: ({ color }) => <Text style={{ color, fontSize: 20 }}>🤖</Text> }} />
            {isAuthenticated && (
                <>
                    <Tab.Screen name="İlan Ver" component={CreateListingScreen}
                        options={{ tabBarIcon: ({ color }) => <Text style={{ color, fontSize: 20 }}>📝</Text> }} />
                    <Tab.Screen name="Favoriler" component={FavoritesScreen}
                        options={{ tabBarIcon: ({ color }) => <Text style={{ color, fontSize: 20 }}>❤️</Text> }} />
                </>
            )}
        </Tab.Navigator>
    );
}

export default function AppNavigator() {
    const { isAuthenticated } = useAuth();

    return (
        <NavigationContainer>
            <Stack.Navigator screenOptions={screenOptions}>
                <Stack.Screen name="Ana Sayfa" component={HomeTabs} options={{ headerShown: false }} />
                <Stack.Screen name="İlan Detayı" component={ListingDetailScreen} />
                {!isAuthenticated && (
                    <>
                        <Stack.Screen name="Giriş" component={LoginScreen} />
                        <Stack.Screen name="Kayıt" component={RegisterScreen} />
                    </>
                )}
            </Stack.Navigator>
        </NavigationContainer>
    );
}
