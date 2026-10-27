import axios from 'axios';
import './style.css';

// Axios client: mọi request đều đi đến ASP.NET Core API.
const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  timeout: 10000,
});

const $ = (selector) => document.querySelector(selector);
const productList = $('#product-list');
const form = $('#product-form');
let products = [];
let editingId = null;
let notificationTimeout;

const formatVND = (value) => new Intl.NumberFormat('vi-VN').format(value) + ' ₫';
const formatNumber = (value) => new Intl.NumberFormat('vi-VN').format(value);

function notify(message, kind = 'success') {
  const node = $('#notification');
  node.textContent = message;
  node.className = `notification ${kind}`;
  node.hidden = false;
  clearTimeout(notificationTimeout);
  notificationTimeout = setTimeout(() => { node.hidden = true; }, 5200);
}

function getErrorMessage(error) {
  if (!error.response) return 'Không kết nối được backend. Hãy mở Terminal chạy API ở cổng 5050 rồi thử lại.';
  const body = error.response.data;
  if (typeof body?.message === 'string') return body.message;
  if (body?.errors && typeof body.errors === 'object') {
    return Object.values(body.errors).flat().join(' ');
  }
  return `Yêu cầu thất bại (HTTP ${error.response.status}).`;
}

function el(tag, className, text) {
  const node = document.createElement(tag);
  if (className) node.className = className;
  if (text !== undefined) node.textContent = String(text);
  return node;
}

function renderProducts() {
  const keyword = $('#search-input').value.trim().toLocaleLowerCase('vi-VN');
  const filtered = products.filter((product) =>
    [product.name, product.description || ''].some((s) => s.toLocaleLowerCase('vi-VN').includes(keyword))
  );

  $('#product-count').textContent = formatNumber(products.length);
  $('#quantity-count').textContent = formatNumber(products.reduce((sum, p) => sum + p.quantity, 0));
  $('#inventory-value').textContent = formatVND(products.reduce((sum, p) => sum + p.price * p.quantity, 0));
  productList.replaceChildren();

  if (filtered.length === 0) {
    const row = el('tr');
    const cell = el('td', 'placeholder-row', keyword ? 'Không có sản phẩm phù hợp.' : 'Chưa có sản phẩm nào. Hãy thêm ở biểu mẫu bên trái!');
    cell.colSpan = 4;
    row.append(cell);
    productList.append(row);
    return;
  }

  filtered.forEach((product) => {
    const row = el('tr');
    const firstCell = el('td');
    const productInfo = el('div', 'product-info');
    const avatar = el('span', 'product-avatar', product.name.charAt(0).toLocaleUpperCase('vi-VN'));
    const details = el('div');
    details.append(el('strong', 'product-name', product.name), el('small', 'product-description', product.description || 'Không có mô tả'));
    productInfo.append(avatar, details);
    firstCell.append(productInfo);

    const priceCell = el('td', 'price-cell', formatVND(product.price));
    const quantityCell = el('td');
    quantityCell.append(el('span', 'quantity-badge', product.quantity));

    const actionsCell = el('td');
    const actionWrap = el('div', 'table-actions');
    const editButton = el('button', 'edit-button', 'Sửa');
    editButton.type = 'button';
    editButton.setAttribute('aria-label', `Sửa ${product.name}`);
    editButton.addEventListener('click', () => editProduct(product.id));

    const deleteButton = el('button', 'delete-button', 'Xóa');
    deleteButton.type = 'button';
    deleteButton.setAttribute('aria-label', `Xóa ${product.name}`);
    deleteButton.addEventListener('click', () => deleteProduct(product.id, product.name));

    actionWrap.append(editButton, deleteButton);
    actionsCell.append(actionWrap);
    row.append(firstCell, priceCell, quantityCell, actionsCell);
    productList.append(row);
  });
}

// GET /api/products
async function loadProducts() {
  try {
    const response = await api.get('/products');
    products = response.data;
    renderProducts();
  } catch (error) {
    products = [];
    renderProducts();
    notify(getErrorMessage(error), 'error');
  }
}

function resetForm() {
  editingId = null;
  form.reset();
  $('#form-title').textContent = 'Thêm sản phẩm';
  $('#submit-label').textContent = '+ Thêm sản phẩm';
  $('#cancel-button').hidden = true;
}

// GET /api/products/{id} - tải dữ liệu mới nhất trước khi sửa.
async function editProduct(id) {
  try {
    const { data: product } = await api.get(`/products/${id}`);
    editingId = product.id;
    $('#name').value = product.name;
    $('#description').value = product.description || '';
    $('#price').value = product.price;
    $('#quantity').value = product.quantity;
    $('#form-title').textContent = `Sửa sản phẩm #${id}`;
    $('#submit-label').textContent = '✓ Lưu thay đổi';
    $('#cancel-button').hidden = false;
    $('#name').focus();
    if (window.innerWidth < 850) form.scrollIntoView({ behavior: 'smooth', block: 'start' });
  } catch (error) {
    notify(getErrorMessage(error), 'error');
  }
}

// POST /api/products hoặc PUT /api/products/{id}
form.addEventListener('submit', async (event) => {
  event.preventDefault();
  const name = $('#name').value.trim();
  const description = $('#description').value.trim();
  const price = Number($('#price').value);
  const quantity = Number($('#quantity').value);

  if (!name || !Number.isSafeInteger(price) || price < 0 || !Number.isInteger(quantity) || quantity < 0) {
    notify('Vui lòng nhập tên, giá và số lượng hợp lệ.', 'error');
    return;
  }

  const input = { name, description, price, quantity };
  const button = $('#submit-button');
  button.disabled = true;
  try {
    if (editingId === null) {
      await api.post('/products', input);
      notify('Đã thêm sản phẩm thành công (POST).');
    } else {
      await api.put(`/products/${editingId}`, input);
      notify('Đã cập nhật sản phẩm thành công (PUT).');
    }
    resetForm();
    await loadProducts();
  } catch (error) {
    notify(getErrorMessage(error), 'error');
  } finally {
    button.disabled = false;
  }
});

// DELETE /api/products/{id}
async function deleteProduct(id, name) {
  if (!window.confirm(`Bạn có chắc muốn xóa sản phẩm "${name}" không?`)) return;
  try {
    await api.delete(`/products/${id}`);
    if (editingId === id) resetForm();
    notify('Đã xóa sản phẩm thành công (DELETE).');
    await loadProducts();
  } catch (error) {
    notify(getErrorMessage(error), 'error');
  }
}

$('#search-input').addEventListener('input', renderProducts);
$('#reload-button').addEventListener('click', loadProducts);
$('#cancel-button').addEventListener('click', resetForm);
loadProducts();
