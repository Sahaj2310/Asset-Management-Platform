import React, { useState, useEffect } from 'react';
import {
  Box,
  Button,
  Card,
  CardContent,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Grid,
  IconButton,
  TextField,
  Typography,
  MenuItem,
  CircularProgress,
} from '@mui/material';
import { Add as AddIcon, Edit as EditIcon } from '@mui/icons-material';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import { companyService, Company, CompanyRegistrationRequest } from '../../services/companyService';
import { locationService, LocationDto } from '../../services/locationService';
import { useSnackbar } from 'notistack';

const validationSchema = Yup.object({
  name: Yup.string().required('Company name is required'),
  description: Yup.string().required('Description is required'),
  address: Yup.string().required('Address is required'),
  countryId: Yup.number().required('Country is required'),
  stateId: Yup.number().required('State is required'),
  cityId: Yup.number().required('City is required'),
  zipCode: Yup.string().required('Zip code is required'),
  phone: Yup.string().required('Phone is required'),
  email: Yup.string().email('Invalid email').required('Email is required'),
  website: Yup.string().url('Invalid URL').required('Website is required'),
});

const Companies: React.FC = () => {
  const [company, setCompany] = useState<Company | null>(null);
  const [open, setOpen] = useState(false);
  const [loading, setLoading] = useState(true);
  const [countries, setCountries] = useState<LocationDto[]>([]);
  const [states, setStates] = useState<LocationDto[]>([]);
  const [cities, setCities] = useState<LocationDto[]>([]);
  const { enqueueSnackbar } = useSnackbar();

  const formik = useFormik<CompanyRegistrationRequest>({
    initialValues: {
      name: '',
      description: '',
      address: '',
      countryId: 0,
      stateId: 0,
      cityId: 0,
      zipCode: '',
      phone: '',
      email: '',
      website: '',
    },
    validationSchema,
    onSubmit: async (values) => {
      try {
        const formData = new FormData();
        Object.entries(values).forEach(([key, value]) => {
          formData.append(key, value.toString());
        });

        if (company) {
          await companyService.updateCompany(formData);
          enqueueSnackbar('Company updated successfully', { variant: 'success' });
        } else {
          await companyService.registerCompany(formData);
          enqueueSnackbar('Company registered successfully', { variant: 'success' });
        }
        handleClose();
        fetchCompany();
      } catch (error) {
        enqueueSnackbar('Error saving company', { variant: 'error' });
      }
    },
  });

  const fetchCompany = async () => {
    try {
      setLoading(true);
      const data = await companyService.getCompany();
      setCompany(data);
      formik.setValues({
        name: data.name,
        description: data.description,
        address: data.address,
        countryId: data.countryId,
        stateId: data.stateId,
        cityId: data.cityId,
        zipCode: data.zipCode,
        phone: data.phone,
        email: data.email,
        website: data.website,
      });
    } catch (error) {
      enqueueSnackbar('Error fetching company', { variant: 'error' });
    } finally {
      setLoading(false);
    }
  };

  const fetchLocations = async () => {
    try {
      const countriesData = await locationService.getCountries();
      setCountries(countriesData);
    } catch (error) {
      enqueueSnackbar('Error fetching locations', { variant: 'error' });
    }
  };

  const handleCountryChange = async (countryId: number) => {
    try {
      const statesData = await locationService.getStates(countryId);
      setStates(statesData);
      setCities([]);
      formik.setFieldValue('stateId', 0);
      formik.setFieldValue('cityId', 0);
    } catch (error) {
      enqueueSnackbar('Error fetching states', { variant: 'error' });
    }
  };

  const handleStateChange = async (stateId: number) => {
    try {
      const citiesData = await locationService.getCities(stateId);
      setCities(citiesData);
      formik.setFieldValue('cityId', 0);
    } catch (error) {
      enqueueSnackbar('Error fetching cities', { variant: 'error' });
    }
  };

  useEffect(() => {
    fetchCompany();
    fetchLocations();
  }, []);

  const handleOpen = () => setOpen(true);
  const handleClose = () => {
    setOpen(false);
    formik.resetForm();
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">Company Profile</Typography>
        <Button
          variant="contained"
          color="primary"
          startIcon={company ? <EditIcon /> : <AddIcon />}
          onClick={handleOpen}
        >
          {company ? 'Edit Company' : 'Register Company'}
        </Button>
      </Box>

      {company && (
        <Card>
          <CardContent>
            <Grid container spacing={3}>
              <Grid item xs={12} md={6}>
                <Typography variant="h6">Company Information</Typography>
                <Typography>Name: {company.name}</Typography>
                <Typography>Description: {company.description}</Typography>
                <Typography>Email: {company.email}</Typography>
                <Typography>Phone: {company.phone}</Typography>
                <Typography>Website: {company.website}</Typography>
              </Grid>
              <Grid item xs={12} md={6}>
                <Typography variant="h6">Address</Typography>
                <Typography>{company.address}</Typography>
                <Typography>
                  {company.zipCode}
                </Typography>
              </Grid>
            </Grid>
          </CardContent>
        </Card>
      )}

      <Dialog open={open} onClose={handleClose} maxWidth="md" fullWidth>
        <DialogTitle>{company ? 'Edit Company' : 'Register Company'}</DialogTitle>
        <form onSubmit={formik.handleSubmit}>
          <DialogContent>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <TextField
                  fullWidth
                  name="name"
                  label="Company Name"
                  value={formik.values.name}
                  onChange={formik.handleChange}
                  error={formik.touched.name && Boolean(formik.errors.name)}
                  helperText={formik.touched.name && formik.errors.name}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  fullWidth
                  multiline
                  rows={3}
                  name="description"
                  label="Description"
                  value={formik.values.description}
                  onChange={formik.handleChange}
                  error={formik.touched.description && Boolean(formik.errors.description)}
                  helperText={formik.touched.description && formik.errors.description}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  fullWidth
                  name="address"
                  label="Address"
                  value={formik.values.address}
                  onChange={formik.handleChange}
                  error={formik.touched.address && Boolean(formik.errors.address)}
                  helperText={formik.touched.address && formik.errors.address}
                />
              </Grid>
              <Grid item xs={12} md={4}>
                <TextField
                  fullWidth
                  select
                  name="countryId"
                  label="Country"
                  value={formik.values.countryId}
                  onChange={(e) => {
                    formik.handleChange(e);
                    handleCountryChange(Number(e.target.value));
                  }}
                  error={formik.touched.countryId && Boolean(formik.errors.countryId)}
                  helperText={formik.touched.countryId && formik.errors.countryId}
                >
                  {countries.map((country) => (
                    <MenuItem key={country.id} value={country.id}>
                      {country.name}
                    </MenuItem>
                  ))}
                </TextField>
              </Grid>
              <Grid item xs={12} md={4}>
                <TextField
                  fullWidth
                  select
                  name="stateId"
                  label="State"
                  value={formik.values.stateId}
                  onChange={(e) => {
                    formik.handleChange(e);
                    handleStateChange(Number(e.target.value));
                  }}
                  error={formik.touched.stateId && Boolean(formik.errors.stateId)}
                  helperText={formik.touched.stateId && formik.errors.stateId}
                >
                  {states.map((state) => (
                    <MenuItem key={state.id} value={state.id}>
                      {state.name}
                    </MenuItem>
                  ))}
                </TextField>
              </Grid>
              <Grid item xs={12} md={4}>
                <TextField
                  fullWidth
                  select
                  name="cityId"
                  label="City"
                  value={formik.values.cityId}
                  onChange={formik.handleChange}
                  error={formik.touched.cityId && Boolean(formik.errors.cityId)}
                  helperText={formik.touched.cityId && formik.errors.cityId}
                >
                  {cities.map((city) => (
                    <MenuItem key={city.id} value={city.id}>
                      {city.name}
                    </MenuItem>
                  ))}
                </TextField>
              </Grid>
              <Grid item xs={12} md={4}>
                <TextField
                  fullWidth
                  name="zipCode"
                  label="Zip Code"
                  value={formik.values.zipCode}
                  onChange={formik.handleChange}
                  error={formik.touched.zipCode && Boolean(formik.errors.zipCode)}
                  helperText={formik.touched.zipCode && formik.errors.zipCode}
                />
              </Grid>
              <Grid item xs={12} md={4}>
                <TextField
                  fullWidth
                  name="phone"
                  label="Phone"
                  value={formik.values.phone}
                  onChange={formik.handleChange}
                  error={formik.touched.phone && Boolean(formik.errors.phone)}
                  helperText={formik.touched.phone && formik.errors.phone}
                />
              </Grid>
              <Grid item xs={12} md={4}>
                <TextField
                  fullWidth
                  name="email"
                  label="Email"
                  value={formik.values.email}
                  onChange={formik.handleChange}
                  error={formik.touched.email && Boolean(formik.errors.email)}
                  helperText={formik.touched.email && formik.errors.email}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  fullWidth
                  name="website"
                  label="Website"
                  value={formik.values.website}
                  onChange={formik.handleChange}
                  error={formik.touched.website && Boolean(formik.errors.website)}
                  helperText={formik.touched.website && formik.errors.website}
                />
              </Grid>
            </Grid>
          </DialogContent>
          <DialogActions>
            <Button onClick={handleClose}>Cancel</Button>
            <Button type="submit" variant="contained" color="primary">
              {company ? 'Update' : 'Register'}
            </Button>
          </DialogActions>
        </form>
      </Dialog>
    </Box>
  );
};

export default Companies; 