import React, { useState, useEffect } from 'react';
import { Form } from 'react-bootstrap';
import axios from 'axios';

interface Location {
    id: number;
    name: string;
}

interface LocationSelectorProps {
    onLocationChange: (countryId: number, stateId: number, cityId: number) => void;
    initialCountryId?: number;
    initialStateId?: number;
    initialCityId?: number;
}

const LocationSelector: React.FC<LocationSelectorProps> = ({
    onLocationChange,
    initialCountryId,
    initialStateId,
    initialCityId
}) => {
    const [countries, setCountries] = useState<Location[]>([]);
    const [states, setStates] = useState<Location[]>([]);
    const [cities, setCities] = useState<Location[]>([]);
    
    const [selectedCountryId, setSelectedCountryId] = useState<number | ''>(initialCountryId || '');
    const [selectedStateId, setSelectedStateId] = useState<number | ''>(initialStateId || '');
    const [selectedCityId, setSelectedCityId] = useState<number | ''>(initialCityId || '');

    useEffect(() => {
        // Load countries on component mount
        const fetchCountries = async () => {
            try {
                const response = await axios.get('/api/locations/countries');
                setCountries(response.data);
            } catch (error) {
                console.error('Error fetching countries:', error);
            }
        };
        fetchCountries();
    }, []);

    useEffect(() => {
        // Load states when country changes
        const fetchStates = async () => {
            if (selectedCountryId) {
                try {
                    const response = await axios.get(`/api/locations/states?countryId=${selectedCountryId}`);
                    setStates(response.data);
                    setSelectedStateId(''); // Reset state selection
                    setSelectedCityId(''); // Reset city selection
                    setCities([]); // Clear cities
                } catch (error) {
                    console.error('Error fetching states:', error);
                }
            } else {
                setStates([]);
                setSelectedStateId('');
                setSelectedCityId('');
                setCities([]);
            }
        };
        fetchStates();
    }, [selectedCountryId]);

    useEffect(() => {
        // Load cities when state changes
        const fetchCities = async () => {
            if (selectedStateId) {
                try {
                    const response = await axios.get(`/api/locations/cities?stateId=${selectedStateId}`);
                    setCities(response.data);
                    setSelectedCityId(''); // Reset city selection
                } catch (error) {
                    console.error('Error fetching cities:', error);
                }
            } else {
                setCities([]);
                setSelectedCityId('');
            }
        };
        fetchCities();
    }, [selectedStateId]);

    useEffect(() => {
        // Notify parent component of changes
        if (selectedCountryId && selectedStateId && selectedCityId) {
            onLocationChange(
                Number(selectedCountryId),
                Number(selectedStateId),
                Number(selectedCityId)
            );
        }
    }, [selectedCountryId, selectedStateId, selectedCityId, onLocationChange]);

    return (
        <div className="location-selector">
            <Form.Group className="mb-3">
                <Form.Label>Country</Form.Label>
                <Form.Select
                    value={selectedCountryId}
                    onChange={(e) => setSelectedCountryId(e.target.value ? Number(e.target.value) : '')}
                >
                    <option value="">Select Country</option>
                    {countries.map((country) => (
                        <option key={country.id} value={country.id}>
                            {country.name}
                        </option>
                    ))}
                </Form.Select>
            </Form.Group>

            <Form.Group className="mb-3">
                <Form.Label>State</Form.Label>
                <Form.Select
                    value={selectedStateId}
                    onChange={(e) => setSelectedStateId(e.target.value ? Number(e.target.value) : '')}
                    disabled={!selectedCountryId}
                >
                    <option value="">Select State</option>
                    {states.map((state) => (
                        <option key={state.id} value={state.id}>
                            {state.name}
                        </option>
                    ))}
                </Form.Select>
            </Form.Group>

            <Form.Group className="mb-3">
                <Form.Label>City</Form.Label>
                <Form.Select
                    value={selectedCityId}
                    onChange={(e) => setSelectedCityId(e.target.value ? Number(e.target.value) : '')}
                    disabled={!selectedStateId}
                >
                    <option value="">Select City</option>
                    {cities.map((city) => (
                        <option key={city.id} value={city.id}>
                            {city.name}
                        </option>
                    ))}
                </Form.Select>
            </Form.Group>
        </div>
    );
};

export default LocationSelector; 