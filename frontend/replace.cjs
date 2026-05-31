const fs = require('fs');
const path = require('path');

function walk(dir) {
  let results = [];
  const list = fs.readdirSync(dir);
  list.forEach(file => {
    file = path.join(dir, file);
    const stat = fs.statSync(file);
    if (stat && stat.isDirectory()) {
      results = results.concat(walk(file));
    } else if (file.endsWith('.vue') || file.endsWith('.js')) {
      results.push(file);
    }
  });
  return results;
}

const files = walk('./src');
let changedCount = 0;

files.forEach(file => {
  let content = fs.readFileSync(file, 'utf8');
  let original = content;

  // Replace standard string 'http://localhost:5072/api/...' -> import.meta.env.VITE_API_BASE_URL + '/...'
  content = content.replace(/'http:\/\/localhost:5072\/api([^']*)'/g, "import.meta.env.VITE_API_BASE_URL + '$1'");

  // Replace template literals `http://localhost:5072/api/...` -> `${import.meta.env.VITE_API_BASE_URL}/...`
  content = content.replace(/`http:\/\/localhost:5072\/api([^`]*)`/g, "\\`${import.meta.env.VITE_API_BASE_URL}$1\\`");

  // Replace image URL base 'http://localhost:5072' -> import.meta.env.VITE_BASE_URL
  content = content.replace(/'http:\/\/localhost:5072'/g, "import.meta.env.VITE_BASE_URL");
  content = content.replace(/`http:\/\/localhost:5072\$\{([^}]+)\}`/g, "\\`${import.meta.env.VITE_BASE_URL}\\${$1}\\`");

  if (content !== original) {
    fs.writeFileSync(file, content, 'utf8');
    console.log('Updated:', file);
    changedCount++;
  }
});
console.log('Done! Files updated:', changedCount);
