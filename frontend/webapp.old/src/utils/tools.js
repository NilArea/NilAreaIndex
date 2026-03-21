export function copyToClipboard(text) {
  return navigator.clipboard.writeText(text);
}

export function dayOfYear(date) {
  return Math.floor((date - new Date(date.getFullYear(), 0, 0)) / 1000 / 60 / 60 / 24);
}

export function gray(r, g, b) {
  return 0.2126 * r + 0.7152 * g + b;
}

export function parseQuery(url) {
  let q = {};
  url.replace(/([^?&=]+)=([^&]+)/g, (_, k, v) => (q[k] = v));
  return q;
}

export function pick(obj, ...props) {
  return Object.fromEntries(Object.entries(obj).filter(([k]) => props.includes(k)));
}

export function randomColor() {
  return Math.floor(Math.random() * 0xffffff).toString(16).padStart(6, '0');
}

export function randomString() {
  return Math.random().toString(36).slice(2);
}

export function removeTag(fragment) {
  return new DOMParser().parseFromString(fragment, 'text/html').body.textContent || '';
}

export function sleep(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}
