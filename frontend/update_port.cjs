const fs = require('fs');
const path = require('path');

const dir = './src/screens';
const files = fs.readdirSync(dir);

files.forEach(file => {
    if (file.endsWith('.vue')) {
        const filePath = path.join(dir, file);
        let content = fs.readFileSync(filePath, 'utf8');
        if (content.includes('http://localhost:5196')) {
            content = content.replace(/http:\/\/localhost:5196/g, 'http://localhost:5072');
            fs.writeFileSync(filePath, content, 'utf8');
            console.log(`Updated ${file}`);
        }
    }
});
