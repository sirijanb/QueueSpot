window.locationPermission = {
    checkPermission: async function () {
        if (!navigator.permissions) {
            return 'prompt'; // Permissions API not supported
        }

        try {
            const result = await navigator.permissions.query({ name: 'geolocation' });
            return result.state; // 'granted', 'denied', or 'prompt'
        } catch (error) {
            return 'prompt';
        }
    },

    requestPermission: function () {
        return new Promise((resolve, reject) => {
            if (!navigator.geolocation) {
                reject('Geolocation not supported');
                return;
            }

            navigator.geolocation.getCurrentPosition(
                position => resolve({
                    latitude: position.coords.latitude,
                    longitude: position.coords.longitude,
                    accuracy: position.coords.accuracy
                }),
                error => {
                    switch (error.code) {
                        case error.PERMISSION_DENIED:
                            reject('Location permission denied by user');
                            break;
                        case error.POSITION_UNAVAILABLE:
                            reject('Location information unavailable');
                            break;
                        case error.TIMEOUT:
                            reject('Location request timed out');
                            break;
                        default:
                            reject('An unknown error occurred');
                    }
                },
                {
                    enableHighAccuracy: true,
                    timeout: 10000,
                    maximumAge: 0
                }
            );
        });
    }
};