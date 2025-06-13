import React, { useState, useEffect } from 'react';
import { Form, Button, Alert } from 'react-bootstrap';
import { siteService, SiteResponse } from '../services/siteService';
import { locationService, CreateLocationRequest, LocationResponse } from '../services/locationService';
import { companyService } from '../services/companyService';
import { useAuth } from '../hooks/useAuth';

interface LocationFormProps {
  onLocationCreated?: (location: LocationResponse) => void;
}

const LocationForm: React.FC<LocationFormProps> = ({ onLocationCreated }) => {
  const { user } = useAuth();
  const [locationName, setLocationName] = useState<string>('');
  const [selectedSiteId, setSelectedSiteId] = useState<string>('');
  const [sites, setSites] = useState<SiteResponse[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [companyId, setCompanyId] = useState<string | null>(null);

  useEffect(() => {
    const fetchCompanyAndSites = async () => {
      if (user) {
        try {
          setLoading(true);
          const company = await companyService.getCompanyByUserId(user.id);
          if (company) {
            setCompanyId(company.id);
            const fetchedSites = await siteService.getSitesByCompanyId(company.id);
            setSites(fetchedSites);
          } else {
            setError('User does not have an associated company. Please register a company first.');
          }
        } catch (err: any) {
          setError(err.message || 'Failed to load company or sites.');
        } finally {
          setLoading(false);
        }
      }
    };
    fetchCompanyAndSites();
  }, [user]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccess(null);

    if (!locationName.trim()) {
      setError('Location name is required.');
      return;
    }
    if (!selectedSiteId) {
      setError('Please select a site.');
      return;
    }
    if (!companyId) {
      setError('Company ID is missing. Cannot create location.');
      return;
    }

    setLoading(true);
    try {
      const newLocation: CreateLocationRequest = {
        name: locationName,
        siteId: selectedSiteId,
        companyId: companyId,
      };

      const createdLocation = await locationService.createLocation(newLocation);
      setSuccess('Location created successfully!');
      setLocationName('');
      setSelectedSiteId('');
      if (onLocationCreated) {
        onLocationCreated(createdLocation);
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to create location.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="location-form-container">
      <h2>Add New Location</h2>
      <Form onSubmit={handleSubmit}>
        {error && <Alert variant="danger">{error}</Alert>}
        {success && <Alert variant="success">{success}</Alert>}

        <Form.Group className="mb-3">
          <Form.Label>Location *</Form.Label>
          <Form.Control
            type="text"
            placeholder="Enter location name"
            value={locationName}
            onChange={(e) => setLocationName(e.target.value)}
            required
            disabled={loading || !companyId}
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Select Site *</Form.Label>
          <Form.Select
            value={selectedSiteId}
            onChange={(e) => setSelectedSiteId(e.target.value)}
            required
            disabled={loading || sites.length === 0}
          >
            <option value="">Select Site</option>
            {sites.map((site) => (
              <option key={site.id} value={site.id}>
                {site.name}
              </option>
            ))}
          </Form.Select>
          {sites.length === 0 && !loading && !error && (
            <Form.Text className="text-muted">
              No sites found. Please create a site first.
            </Form.Text>
          )}
        </Form.Group>

        <Button variant="primary" type="submit" disabled={loading || !companyId || sites.length === 0}>
          {loading ? 'Creating...' : 'Add Location'}
        </Button>
      </Form>
    </div>
  );
};

export default LocationForm; 