import React, { useState } from 'react';
import './CompanyRegistrationForm.css'; // We'll create this CSS file next

function CompanyRegistrationForm({ onRegistrationSuccess }) {
  const [formData, setFormData] = useState({
    companyName: '',
    country: '',
    state: '',
    city: '',
    zipCode: '',
    address: '',
    financialYearMonth: 1, // Default to January
    financialYearDay: 1,   // Default to 1st
    currency: '',
    logo: null, // For file input
  });

  const [logoPreview, setLogoPreview] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: value,
    });
  };

  const handleFileChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setFormData({
        ...formData,
        logo: file,
      });
      
      // Create a preview URL for the selected image
      const reader = new FileReader();
      reader.onloadend = () => {
        setLogoPreview(reader.result);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setSuccess(false);
    setLoading(true);

    const token = localStorage.getItem('jwtToken');

    if (!token) {
      setError('Authentication token not found. Please log in again.');
      setLoading(false);
      return;
    }

    // Create FormData to handle file upload
    const formDataToSubmit = new FormData();
    formDataToSubmit.append('companyName', formData.companyName);
    formDataToSubmit.append('country', formData.country);
    formDataToSubmit.append('state', formData.state);
    formDataToSubmit.append('city', formData.city);
    formDataToSubmit.append('zipCode', formData.zipCode);
    formDataToSubmit.append('address', formData.address);
    formDataToSubmit.append('financialYearMonth', formData.financialYearMonth);
    formDataToSubmit.append('financialYearDay', formData.financialYearDay);
    formDataToSubmit.append('currency', formData.currency);
    if (formData.logo) {
      formDataToSubmit.append('logo', formData.logo);
    }

    try {
      const response = await fetch('/api/Company/register', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
        },
        body: formDataToSubmit,
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Company registration failed.');
      }

      const result = await response.json();
      setSuccess(true);
      if (onRegistrationSuccess) {
        onRegistrationSuccess(result);
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  // Static options for dropdowns (replace with dynamic data if needed)
  const countries = ['Select Country', 'USA', 'Canada', 'UK', 'India']; // Example countries
  const states = { // Example states by country
    'Select Country': ['Select State'],
    USA: ['Select State', 'California', 'Texas', 'New York'],
    Canada: ['Select State', 'Ontario', 'Quebec', 'British Columbia'],
    UK: ['Select State', 'England', 'Scotland', 'Wales'],
    India: ['Select State', 'Maharashtra', 'Delhi', 'Karnataka'],
  };
  const cities = { // Example cities by state
     'Select State': ['Select City'],
    California: ['Select City', 'Los Angeles', 'San Francisco'],
    Texas: ['Select City', 'Houston', 'Dallas'],
    NewYork: ['Select City', 'New York City', 'Buffalo'],
     Ontario: ['Select City', 'Toronto', 'Ottawa'],
     Quebec: ['Select City', 'Montreal', 'Quebec City'],
     'British Columbia': ['Select City', 'Vancouver', 'Victoria'],
     England: ['Select City', 'London', 'Manchester'],
     Scotland: ['Select City', 'Edinburgh', 'Glasgow'],
     Wales: ['Select City', 'Cardiff', 'Swansea'],
     Maharashtra: ['Select City', 'Mumbai', 'Pune'],
     Delhi: ['Select City', 'New Delhi'],
     Karnataka: ['Select City', 'Bangalore', 'Mysuru'],
     'Select City': ['Select City'], // Default
  };
    const months = Array.from({ length: 12 }, (_, i) => i + 1); // 1 to 12
    const days = Array.from({ length: 31 }, (_, i) => i + 1);   // 1 to 31
  const currencies = ['Indian Rupee (INR) (₹)', 'US Dollar (USD) ($)', 'British Pound (GBP) (£)']; // Example currencies


  return (
    <div className="company-registration-container">
      <h2>Company Registration</h2>
      <form onSubmit={handleSubmit}>
        {/* Logo Upload Section */}
        <div className="form-group logo-upload">
            <label>Company Logo</label>
            <div className="logo-upload-container">
                <div className="logo-preview" style={{ backgroundImage: logoPreview ? `url(${logoPreview})` : 'none' }}>
                    {!logoPreview && (
                        <div className="logo-placeholder">
                            <span>Click to upload logo</span>
                            <span className="logo-hint">Allowed: PNG, JPG, JPEG</span>
                        </div>
                    )}
                    <input 
                        type="file" 
                        accept=".png,.jpg,.jpeg" 
                        onChange={handleFileChange}
                        className="logo-input"
                    />
                </div>
                {logoPreview && (
                    <button 
                        type="button" 
                        className="remove-logo"
                        onClick={() => {
                            setLogoPreview(null);
                            setFormData(prev => ({ ...prev, logo: null }));
                        }}
                    >
                        Remove Logo
                    </button>
                )}
            </div>
        </div>

        <h3>Details</h3>

        {/* Company Name */}
        <div className="form-group">
          <label htmlFor="companyName">Company Name *</label>
          <input
            type="text"
            id="companyName"
            name="companyName"
            value={formData.companyName}
            onChange={handleInputChange}
            required
          />
        </div>

        {/* Country */}
        <div className="form-row">
            <div className="form-group">
                <label htmlFor="country">Country *</label>
                <select id="country" name="country" value={formData.country} onChange={handleInputChange} required>
                    {countries.map(country => <option key={country} value={country}>{country}</option>)}
                </select>
            </div>

            {/* State */}
            <div className="form-group">
                <label htmlFor="state">State *</label>
                 <select id="state" name="state" value={formData.state} onChange={handleInputChange} required disabled={!formData.country || formData.country === 'Select Country'}>
                    {states[formData.country]?.map(state => <option key={state} value={state}>{state}</option>) || <option value="">Select State</option>}
                </select>
            </div>
        </div>

        {/* City */}
        <div className="form-row">
             <div className="form-group">
                <label htmlFor="city">City *</label>
                 <select id="city" name="city" value={formData.city} onChange={handleInputChange} required disabled={!formData.state || formData.state === 'Select State'}>
                     {cities[formData.state]?.map(city => <option key={city} value={city}>{city}</option>) || <option value="">Select City</option>}
                 </select>
             </div>
             {/* Zip Code */}
             <div className="form-group">
                 <label htmlFor="zipCode">Zip Code *</label>
                 <input
                    type="text"
                    id="zipCode"
                    name="zipCode"
                    value={formData.zipCode}
                    onChange={handleInputChange}
                    required
                 />
             </div>
        </div>


        {/* Address */}
        <div className="form-group">
          <label htmlFor="address">Address *</label>
          <textarea
            id="address"
            name="address"
            value={formData.address}
            onChange={handleInputChange}
            required
            rows="4"
          ></textarea>
        </div>

        {/* Financial Year */}
        <div className="form-row">
             <div className="form-group">
                <label>Finacial Year begins on *</label>
                 <div className="financial-year-selects">
                     <select name="financialYearMonth" value={formData.financialYearMonth} onChange={handleInputChange} required>
                        {months.map(month => <option key={month} value={month}>{new Date(0, month - 1).toLocaleString('en', { month: 'long' })}</option>)}
                     </select>
                     <select name="financialYearDay" value={formData.financialYearDay} onChange={handleInputChange} required>
                         {days.map(day => <option key={day} value={day}>{day}</option>)}
                     </select>
                 </div>
            </div>
             {/* Currency */}
             <div className="form-group">
                <label htmlFor="currency">Currency *</label>
                <select id="currency" name="currency" value={formData.currency} onChange={handleInputChange} required>
                    <option value="">Select Currency</option>
                    {currencies.map(currency => <option key={currency} value={currency}>{currency}</option>)}
                </select>
            </div>
        </div>


        {/* Submission Buttons */}
        <div className="form-actions">
          {error && <p className="error-message">{error}</p>}
          {success && <p className="success-message">Company registered successfully!</p>}
          <button type="button" className="btn-cancel">Cancel</button> {/* Example Cancel button */}
          <button type="submit" disabled={loading}>
            {loading ? 'Registering...' : 'Update'}
          </button>
        </div>
      </form>
    </div>
  );
}

export default CompanyRegistrationForm; 