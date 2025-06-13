import React, { useState, useEffect } from 'react';
import {
  Box,
  Button,
  Card,
  CardContent,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Typography,
  IconButton,
  InputAdornment,
  MenuItem,
  CircularProgress,
  Alert,
} from '@mui/material';
import {
  Add as AddIcon,
  Search as SearchIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  FileDownload as FileDownloadIcon,
} from '@mui/icons-material';
import { DataGrid, GridColDef } from '@mui/x-data-grid';
import { useFormik } from 'formik';
import * as yup from 'yup';
import { siteService, SiteResponse, CreateSiteRequest } from '../../services/siteService';
import { useSnackbar } from 'notistack';
import { useAuth } from '../../hooks/useAuth';
import { companyService } from '../../services/companyService';

const validationSchema = yup.object({
  name: yup.string().required('Site name is required'),
  description: yup.string().required('Description is required'),
  address: yup.string().required('Address is required'),
  countryId: yup.number().required('Country is required'),
  stateId: yup.number().required('State is required'),
  cityId: yup.number().required('City is required'),
  zipCode: yup.string().required('Zip code is required'),
});

const Sites: React.FC = () => {
  const [sites, setSites] = useState<SiteResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [open, setOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedSite, setSelectedSite] = useState<SiteResponse | null>(null);
  const { enqueueSnackbar } = useSnackbar();
  const { user } = useAuth();
  const [companyId, setCompanyId] = useState<string | null>(null);

  const formik = useFormik({
    initialValues: {
      name: '',
      description: '',
      address: '',
      countryId: 0,
      stateId: 0,
      cityId: 0,
      zipCode: '',
    },
    validationSchema,
    onSubmit: async (values) => {
      try {
        if (!companyId) {
          enqueueSnackbar('Company ID not found', { variant: 'error' });
          return;
        }

        if (selectedSite) {
          await siteService.updateSite(selectedSite.id, { ...values, companyId });
          enqueueSnackbar('Site updated successfully', { variant: 'success' });
        } else {
          await siteService.createSite({ ...values, companyId });
          enqueueSnackbar('Site created successfully', { variant: 'success' });
        }
        handleClose();
        fetchSites();
      } catch (error) {
        enqueueSnackbar('Error saving site', { variant: 'error' });
      }
    },
  });

  const fetchSites = async () => {
    try {
      setLoading(true);
      if (companyId) {
        const data = await siteService.getSitesByCompanyId(companyId);
        setSites(data);
      }
    } catch (error) {
      setError('Error fetching sites');
      enqueueSnackbar('Error fetching sites', { variant: 'error' });
    } finally {
      setLoading(false);
    }
  };

  const fetchCompany = async () => {
    if (user) {
      try {
        const company = await companyService.getCompanyByUserId(user.id);
        if (company) {
          setCompanyId(company.id);
        }
      } catch (error) {
        enqueueSnackbar('Error fetching company', { variant: 'error' });
      }
    }
  };

  useEffect(() => {
    fetchCompany();
  }, [user]);

  useEffect(() => {
    if (companyId) {
      fetchSites();
    }
  }, [companyId]);

  const handleOpen = () => {
    setSelectedSite(null);
    formik.resetForm();
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
    setSelectedSite(null);
    formik.resetForm();
  };

  const handleEdit = (site: SiteResponse) => {
    setSelectedSite(site);
    formik.setValues({
      name: site.name,
      description: site.description,
      address: site.address,
      countryId: site.countryId,
      stateId: site.stateId,
      cityId: site.cityId,
      zipCode: site.zipCode,
    });
    setOpen(true);
  };

  const handleDelete = async (id: string) => {
    if (window.confirm('Are you sure you want to delete this site?')) {
      try {
        await siteService.deleteSite(id);
        enqueueSnackbar('Site deleted successfully', { variant: 'success' });
        fetchSites();
      } catch (error) {
        enqueueSnackbar('Error deleting site', { variant: 'error' });
      }
    }
  };

  const handleExport = async () => {
    try {
      const blob = await siteService.exportSitesToExcel();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = 'sites_export.xlsx';
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);
      enqueueSnackbar('Export successful', { variant: 'success' });
    } catch (error) {
      enqueueSnackbar('Error exporting sites', { variant: 'error' });
    }
  };

  const columns: GridColDef[] = [
    { field: 'name', headerName: 'Site Name', flex: 1 },
    { field: 'description', headerName: 'Description', flex: 1 },
    { field: 'address', headerName: 'Address', flex: 1 },
    { field: 'cityName', headerName: 'City', flex: 1 },
    { field: 'stateName', headerName: 'State', flex: 1 },
    { field: 'countryName', headerName: 'Country', flex: 1 },
    { field: 'zipCode', headerName: 'Zip Code', width: 120 },
    {
      field: 'actions',
      headerName: 'Actions',
      width: 120,
      renderCell: (params) => (
        <Box>
          <IconButton
            size="small"
            onClick={() => handleEdit(params.row)}
            color="primary"
          >
            <EditIcon />
          </IconButton>
          <IconButton
            size="small"
            onClick={() => handleDelete(params.row.id)}
            color="error"
          >
            <DeleteIcon />
          </IconButton>
        </Box>
      ),
    },
  ];

  if (loading && sites.length === 0) {
    return (
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          minHeight: '400px',
        }}
      >
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          mb: 3,
        }}
      >
        <Typography variant="h4" sx={{ fontWeight: 600 }}>
          Sites
        </Typography>
        <Box>
          <Button
            variant="outlined"
            startIcon={<FileDownloadIcon />}
            onClick={handleExport}
            sx={{ mr: 2 }}
          >
            Export
          </Button>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={handleOpen}
          >
            Add Site
          </Button>
        </Box>
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <Card elevation={0}>
        <CardContent>
          <Box sx={{ mb: 2 }}>
            <TextField
              fullWidth
              placeholder="Search sites..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon />
                  </InputAdornment>
                ),
              }}
            />
          </Box>
          <DataGrid
            rows={sites}
            columns={columns}
            autoHeight
            pageSize={10}
            rowsPerPageOptions={[10]}
            disableSelectionOnClick
            loading={loading}
            sx={{
              border: 'none',
              '& .MuiDataGrid-cell:focus': {
                outline: 'none',
              },
            }}
          />
        </CardContent>
      </Card>

      <Dialog open={open} onClose={handleClose} maxWidth="md" fullWidth>
        <form onSubmit={formik.handleSubmit}>
          <DialogTitle>
            {selectedSite ? 'Edit Site' : 'Add New Site'}
          </DialogTitle>
          <DialogContent>
            <Box sx={{ mt: 2 }}>
              <TextField
                fullWidth
                name="name"
                label="Site Name"
                value={formik.values.name}
                onChange={formik.handleChange}
                error={formik.touched.name && Boolean(formik.errors.name)}
                helperText={formik.touched.name && formik.errors.name}
                sx={{ mb: 2 }}
              />
              <TextField
                fullWidth
                name="description"
                label="Description"
                value={formik.values.description}
                onChange={formik.handleChange}
                error={formik.touched.description && Boolean(formik.errors.description)}
                helperText={formik.touched.description && formik.errors.description}
                sx={{ mb: 2 }}
              />
              <TextField
                fullWidth
                name="address"
                label="Address"
                value={formik.values.address}
                onChange={formik.handleChange}
                error={formik.touched.address && Boolean(formik.errors.address)}
                helperText={formik.touched.address && formik.errors.address}
                sx={{ mb: 2 }}
              />
              <TextField
                fullWidth
                name="zipCode"
                label="Zip Code"
                value={formik.values.zipCode}
                onChange={formik.handleChange}
                error={formik.touched.zipCode && Boolean(formik.errors.zipCode)}
                helperText={formik.touched.zipCode && formik.errors.zipCode}
                sx={{ mb: 2 }}
              />
            </Box>
          </DialogContent>
          <DialogActions>
            <Button onClick={handleClose}>Cancel</Button>
            <Button type="submit" variant="contained" color="primary">
              {selectedSite ? 'Update' : 'Create'}
            </Button>
          </DialogActions>
        </form>
      </Dialog>
    </Box>
  );
};

export default Sites; 