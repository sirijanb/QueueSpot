window.placesAutocomplete = {
    autocompleteService: null,
    geocoder: null,
    sessionToken: null,

    initialize: function () {
        this.autocompleteService = new google.maps.places.AutocompleteService();
        this.geocoder = new google.maps.Geocoder();
        this.sessionToken = new google.maps.places.AutocompleteSessionToken();
    },

    getPlacePredictions: function (input, countryCode) {
        return new Promise((resolve, reject) => {
            if (!this.autocompleteService) {
                this.initialize();
            }

            const request = {
                input: input,
                sessionToken: this.sessionToken,
                componentRestrictions: { country: countryCode || 'ca' }, // Default to Canada
                types: ['address'] // Can also use ['geocode'] for broader results
            };

            this.autocompleteService.getPlacePredictions(request, (predictions, status) => {
                if (status === google.maps.places.PlacesServiceStatus.OK && predictions) {
                    const results = predictions.map(prediction => ({
                        placeId: prediction.place_id,
                        description: prediction.description,
                        mainText: prediction.structured_formatting.main_text,
                        secondaryText: prediction.structured_formatting.secondary_text
                    }));
                    resolve(results);
                } else if (status === google.maps.places.PlacesServiceStatus.ZERO_RESULTS) {
                    resolve([]);
                } else {
                    reject(`Places Autocomplete failed: ${status}`);
                }
            });
        });
    },

    getLocAddr: function (lat, lng) {
        return new Promise((resolve, reject) => {
            if (!this.geocoder) {
                this.initialize();
            }

            const latlng = new google.maps.LatLng(lat, lng);

            this.geocoder.geocode({ location: latlng }, (results, status) => {
                if (status === 'OK' && results[0]) {
                    const place = results[0];

                    // Extract address components
                    const addressComponents = {};
                    place.address_components.forEach(component => {
                        const type = component.types[0];
                        addressComponents[type] = component.long_name;
                    });

                    const details = {
                        placeId: place.place_id,
                        formattedAddress: place.formatted_address,
                        streetNumber: addressComponents.street_number || '',
                        route: addressComponents.route || '',
                        city: addressComponents.locality || addressComponents.sublocality || '',
                        province: addressComponents.administrative_area_level_1 || '',
                        country: addressComponents.country || '',
                        postalCode: addressComponents.postal_code || '',
                        latitude: place.geometry.location.lat(),
                        longitude: place.geometry.location.lng()
                    };

                    resolve(details);

                    // Reset session token after getting details
                    this.sessionToken = new google.maps.places.AutocompleteSessionToken();
                } else {
                    reject(`Geocoder failed: ${status}`);
                }
            });
        });
    },

    getAddrDetails: function (addr) {
        console.log("getAddrDetails called : " + addr);
        return new Promise((resolve, reject) => {
            if (!this.geocoder) {
                this.initialize();
            }

            this.geocoder.geocode({ address: addr }, (results, status) => {
                console.log([results, status]);
                if (status === 'OK' && results[0]) {
                    const place = results[0];

                    // Extract address components
                    const addressComponents = {};
                    place.address_components.forEach(component => {
                        const type = component.types[0];
                        addressComponents[type] = component.long_name;
                    });

                    const details = {
                        placeId: place.place_id,
                        formattedAddress: place.formatted_address,
                        streetNumber: addressComponents.street_number || '',
                        route: addressComponents.route || '',
                        city: addressComponents.locality || addressComponents.sublocality || '',
                        province: addressComponents.administrative_area_level_1 || '',
                        country: addressComponents.country || '',
                        postalCode: addressComponents.postal_code || '',
                        latitude: place.geometry.location.lat(),
                        longitude: place.geometry.location.lng()
                    };

                    resolve(details);

                    // Reset session token after getting details
                    this.sessionToken = new google.maps.places.AutocompleteSessionToken();
                } else {
                    reject(`Geocoder failed: ${status}`);
                }
            });
        });
    },

    getPlaceDetails: function (placeId) {
        return new Promise((resolve, reject) => {
            if (!this.geocoder) {
                this.initialize();
            }

            this.geocoder.geocode({ placeId: placeId }, (results, status) => {
                if (status === 'OK' && results[0]) {
                    const place = results[0];

                    // Extract address components
                    const addressComponents = {};
                    place.address_components.forEach(component => {
                        const type = component.types[0];
                        addressComponents[type] = component.long_name;
                    });

                    const details = {
                        placeId: placeId,
                        formattedAddress: place.formatted_address,
                        streetNumber: addressComponents.street_number || '',
                        route: addressComponents.route || '',
                        city: addressComponents.locality || addressComponents.sublocality || '',
                        province: addressComponents.administrative_area_level_1 || '',
                        country: addressComponents.country || '',
                        postalCode: addressComponents.postal_code || '',
                        latitude: place.geometry.location.lat(),
                        longitude: place.geometry.location.lng()
                    };

                    resolve(details);

                    // Reset session token after getting details
                    this.sessionToken = new google.maps.places.AutocompleteSessionToken();
                } else {
                    reject(`Geocoder failed: ${status}`);
                }
            });
        });
    },

    getUserLocation: function () {
        return new Promise((resolve, reject) => {
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(
                    position => resolve({
                        latitude: position.coords.latitude,
                        longitude: position.coords.longitude
                    }),
                    error => reject(error.message)
                );
            } else {
                reject("Geolocation not supported");
            }
        });
    }

};
