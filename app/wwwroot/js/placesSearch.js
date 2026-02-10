window.placesSearch = {
  map: null,
  markers: [],
  infoWindow: null,

  initializeMap: function (elementId, latitude, longitude, zoom) {
    const mapOptions = {
      center: { lat: latitude, lng: longitude },
      zoom: zoom,
      mapId: "HOSPITAL_MAIN_MAP", // Required for advanced markers
    };

    this.map = new google.maps.Map(
      document.getElementById(elementId),
      mapOptions,
    );
    this.infoWindow = new google.maps.InfoWindow();

    return true;
  },

  reDrawMap: function () {
    if (!this.map) {
      reject("Map not initialized");
      return;
    }
    google.maps.event.trigger(this.map, "resize");
  },

  clearMarkers: function () {
    this.markers.forEach((marker) => marker.setMap(null));
    this.markers = [];
  },

  searchNearbyPlaces: function (latitude, longitude, radius, keyword, type) {
    return new Promise((resolve, reject) => {
      if (!this.map) {
        reject("Map not initialized");
        return;
      }

      const location = new google.maps.LatLng(latitude, longitude);

      const request = {
        location: location,
        radius: radius,
        keyword: keyword,
        type: type, // e.g., 'hospital', 'restaurant', 'pharmacy'
      };

      const service = new google.maps.places.PlacesService(this.map);

      service.nearbySearch(request, (results, status) => {
        if (status === google.maps.places.PlacesServiceStatus.OK) {
          const places = results.map((place) => ({
            placeId: place.place_id,
            name: place.name,
            address: place.vicinity,
            latitude: place.geometry.location.lat(),
            longitude: place.geometry.location.lng(),
            rating: place.rating || 0,
            userRatingsTotal: place.user_ratings_total || 0,
            isOpen: place.opening_hours?.isOpen() || null,
            types: place.types || [],
            photos: place.photos
              ? place.photos.map((photo) => photo.getUrl({ maxWidth: 400 }))
              : [],
            priceLevel: place.price_level || 0,
            icon: place.icon,
          }));
          resolve(places);
        } else if (
          status === google.maps.places.PlacesServiceStatus.ZERO_RESULTS
        ) {
          resolve([]);
        } else {
          reject(`Places search failed: ${status}`);
        }
      });
    });
  },

  searchNearbyPlacesNew: function (latitude, longitude, radius, includedTypes) {
    return new Promise(async (resolve, reject) => {
      try {
        const { Place } = await google.maps.importLibrary("places");

        const request = {
          fields: [
            "displayName",
            "location",
            "formattedAddress",
            "types",
            "photos",
            "regularOpeningHours"
          ],
          locationRestriction: {
            center: { lat: latitude, lng: longitude },
            radius: radius,
          },
          includedTypes: includedTypes, // e.g., ['hospital', 'pharmacy']
          maxResultCount: 20,
          rankPreference: "DISTANCE",
        };

        const { places } = await Place.searchNearby(request);

        const results = places.map((place) => ({
          placeId: place.id,
          name: place.displayName,
          address: place.formattedAddress,
          latitude: place.location.lat(),
          longitude: place.location.lng(),
          types: place.types || [],
          photos: place.photos
            ? place.photos
                .slice(0, 3)
                .map((photo) => photo.getURI({ maxWidth: 400, maxHeight: 300 }))
                : [],

          openingHours: place.regularOpeningHours ? place.regularOpeningHours.weekdayDescriptions : [],
          isOpen24Hours: this.checkIfOpen24Hours(place)

        }));

        resolve(results);
      } catch (error) {
        reject(`New Places API search failed: ${error.message}`);
      }
    });
  },

  addMarkers: function (places, centerMap) {
    this.clearMarkers();

    if (!places || places.length === 0) return;

    const bounds = new google.maps.LatLngBounds();

    places.forEach((place, index) => {
      const position = { lat: place.latitude, lng: place.longitude };

      const marker = new google.maps.Marker({
        position: position,
        map: this.map,
        title: place.name,
        label: {
          text: (index + 1).toString(),
          color: "white",
          fontSize: "12px",
          fontWeight: "bold",
        },
        animation: google.maps.Animation.DROP,
      });

      const contentString = `
                <div style="max-width: 300px;">
                    <h6 style="margin: 0 0 8px 0; font-weight: bold;">${place.name}</h6>
                    <p style="margin: 0 0 4px 0; font-size: 0.9em; color: #666;">${place.formattedAddress}</p>
                    
                    ${
                      place.photos && place.photos.length > 0
                        ? `
                        <img src="${place.photos[0]}" style="width: 100%; max-height: 150px; object-fit: cover; margin-top: 8px; border-radius: 4px;" />
                    `
                        : ""
                    }
                    
                </div>
            `;

      marker.addListener("click", () => {
        console.log(contentString);
        this.infoWindow.setContent(contentString);
        this.infoWindow.open(this.map, marker);
      });

      this.markers.push(marker);
      bounds.extend(position);
    });

    if (centerMap && places.length > 0) {
      this.map.fitBounds(bounds);
    }
  },

  // addHospitalMarkers: function (hospitals, centerMap) {
  //     this.clearMarkers();

  //     if (!hospitals || hospitals.length === 0) return;

  //     const bounds = new google.maps.LatLngBounds();

  //     hospitals.forEach((hospital, index) => {
  //         const position = { lat: hospital.latitude, lng: hospital.longitude };

  //         const marker = new google.maps.Marker({
  //             position: position,
  //             map: this.map,
  //             title: hospital.name,
  //             label: {
  //                 text: (index + 1).toString(),
  //                 color: 'white',
  //                 fontSize: '12px',
  //                 fontWeight: 'bold'
  //             },
  //             animation: google.maps.Animation.DROP
  //         });

  //         console.log("Hospital adding markers");
  //         console.log(hospital);

  //         const contentString = `
  //             <div style="max-width: 300px;">
  //                 <h6 style="margin: 0 0 8px 0; font-weight: bold;">${hospital.name}</h6>
  //                 <p style="margin: 0 0 4px 0; font-size: 0.9em; color: #666;">${hospital.estimatedWait}</p>

  //                 ${hospital.photos && hospital.photos.length > 0 ? `
  //                     <img src="${hospital.photos[0]}" style="width: 100%; max-height: 150px; object-fit: cover; margin-top: 8px; border-radius: 4px;" />
  //                 ` : ''}

  //             </div>
  //         `;

  //         marker.addListener('click', () => {
  //             console.log(contentString);
  //             this.infoWindow.setContent(contentString);
  //             this.infoWindow.open(this.map, marker);
  //         });

  //         this.markers.push(marker);
  //         bounds.extend(position);
  //     });

  //     if (centerMap && hospitals.length > 0) {
  //         this.map.fitBounds(bounds);
  //     }
  // },
  addHospitalMarkers: function (hospitals, centerMap) {
    if (!this.map) {
      console.warn("Map not initialized yet. Skipping addHospitalMarkers.");
      return;
    }

    this.clearMarkers();

    if (!hospitals || hospitals.length === 0) return;

    const bounds = new google.maps.LatLngBounds();

    hospitals.forEach((hospital, index) => {
      const position = { lat: hospital.latitude, lng: hospital.longitude };

      const marker = new google.maps.Marker({
        position: position,
        map: this.map,
        title: hospital.name,
        label: {
          text: (index + 1).toString(),
          color: "white",
          fontSize: "12px",
          fontWeight: "bold",
        },
        animation: google.maps.Animation.DROP,
      });

      this.markers.push(marker);
      bounds.extend(position);
    });

    if (centerMap && hospitals.length > 0) {
      this.map.fitBounds(bounds);
    }
  },

    addDetailedMarkers: function (hospitals, centerMap) {
        this.clearMarkers();

        if (!hospitals || hospitals.length === 0) return;

        const bounds = new google.maps.LatLngBounds();

        hospitals.forEach((hospital, index) => {
            const position = { lat: hospital.latitude, lng: hospital.longitude };
            let waitTime = `${hospital.estimatedWait}m`;

            console.log(hospital);

            if (hospital.EstimatedWait >= 60) {
                waitTime = `${(hospital.estimatedWait / 60)}h ${hospital.estimatedWait % 60}m`;
            }
            // Create custom HTML marker
            const markerDiv = document.createElement('div');
            markerDiv.className = 'custom-hospital-marker';
            markerDiv.id = "hospital_marker_" + hospital.placeId;
            markerDiv.innerHTML = `
                <div class="marker-content">
                    <div class="marker-layout">
                        <div class="marker-container">
                          <p><b>${hospital.name}</b></p>
                          <p><span>${waitTime}</span></p>
                          </div>
                    </div>
                    <div class="marker-icon">
                        
                      
                    </div>
                    <div class="marker-label">${index + 1}</div>
                </div>
            `;

            // Create advanced marker with HTML content
            const marker = new google.maps.marker.AdvancedMarkerElement({
                map: this.map,
                position: position,
                content: markerDiv,
                title: hospital.name
            });

            // Add click listener for info window
            const contentString = this.createInfoWindowContent(hospital, index);
            marker.addEventListener('click', (evt) => {
                //this.onClickMarker(hospital);
                console.log(hospital);
                console.log("hospital_card_" + hospital.placeID);
                this.setCentre(hospital.latitude, hospital.longitude);
                document.querySelector("#hospital_card_" + hospital.placeID).click();
                document.querySelector("#hospital_card_" + hospital.placeID).scrollIntoView();
            });
            /*markerDiv.addEventListener('click', () => {
                this.infoWindow.setContent(contentString);
                this.infoWindow.open(this.map, marker);
                
            });*/

            this.markers.push(marker);
            bounds.extend(position);
        });

        if (centerMap && hospitals.length > 0) {
            this.map.fitBounds(bounds);
        }
    },

    onClickMarker: function (hospital) {
        console.log(hospital);
    },

    createInfoWindowContent: function (hospital, index) {
        return "";
    },

  setCentre: function (latitude, longitude, zoom) {
    if (this.map) {
      this.map.setCenter({ lat: latitude, lng: longitude });
      if (zoom) {
        this.map.setZoom(zoom);
      }
    }
  },

  getUserLocation: function () {
    return new Promise((resolve, reject) => {
      if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(
          (position) =>
            resolve({
              latitude: position.coords.latitude,
              longitude: position.coords.longitude,
            }),
          (error) => reject(error.message),
        );
      } else {
        reject("Geolocation not supported");
      }
    });
  },
};
