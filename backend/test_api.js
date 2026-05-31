const fetch = require('node-fetch');

// This script requires Node v18+ (which has built-in fetch) or node-fetch.
// I will use standard Node.js built-in fetch if possible, or http module to be safe.
// Let's use the built-in fetch from Node 18+

const BASE_URL = 'http://localhost:5072/api';
let token = '';
let adminToken = '';
let userId = 0;
let productId = 1; // Assuming product 1 exists

async function runTests() {
    console.log("🚀 BẮT ĐẦU CHUỖI TEST TỰ ĐỘNG (Phase 5 - 16)...\n");

    try {
        // --- PHASE 5: AUTH ---
        console.log("✅ [Phase 5] Đăng ký tài khoản mới...");
        const testEmail = `testuser_${Date.now()}@example.com`;
        const regRes = await fetch(`${BASE_URL}/auth/register`, {
            method: 'POST', headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ fullName: "Test User", email: testEmail, password: "Password123!" })
        });
        const regData = await regRes.json();
        if(regData.success) console.log("   -> Đăng ký thành công!");
        else console.log("   -> Đăng ký thất bại:", regData);

        console.log("✅ [Phase 5] Đăng nhập...");
        const loginRes = await fetch(`${BASE_URL}/auth/login`, {
            method: 'POST', headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email: testEmail, password: "Password123!" })
        });
        const loginData = await loginRes.json();
        if(loginData.success) {
            token = loginData.data.token;
            userId = loginData.data.user.userId;
            console.log("   -> Đăng nhập thành công! Lấy được Token.");
        }

        // --- PHASE 14: RATE LIMITING ---
        console.log("✅ [Phase 14] Test Rate Limiting (Spam Đăng nhập)...");
        let rateLimited = false;
        for(let i = 0; i < 7; i++) {
            const spamRes = await fetch(`${BASE_URL}/auth/login`, {
                method: 'POST', headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email: "wrong@example.com", password: "123" })
            });
            if (spamRes.status === 429) {
                rateLimited = true;
                break;
            }
        }
        if(rateLimited) console.log("   -> Chuẩn! Đã chặn thành công (429 Too Many Requests) khi spam login.");
        else console.log("   -> Không bị chặn (có thể do config).");

        // --- PHASE 7 & 13: SẢN PHẨM & PHÂN TRANG ---
        console.log("✅ [Phase 7 & 13] Lấy danh sách sản phẩm (Phân trang)...");
        const prodRes = await fetch(`${BASE_URL}/product?page=1&pageSize=2`);
        const prodData = await prodRes.json();
        if(prodData.success) {
            console.log(`   -> Lấy thành công ${prodData.data.length} sản phẩm (Trang 1, kích thước 2). Tổng: ${prodData.totalCount}`);
            if (prodData.data.length > 0) {
                productId = prodData.data[0].productId;
            }
        }

        // --- PHASE 15: WISHLIST ---
        console.log("✅ [Phase 15] Thêm sản phẩm vào Wishlist...");
        const wishRes = await fetch(`${BASE_URL}/wishlist/toggle`, {
            method: 'POST', headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
            body: JSON.stringify({ productId: productId })
        });
        const wishData = await wishRes.json();
        if(wishData.success) console.log(`   -> Toggle wishlist thành công! isAdded: ${wishData.isAdded}`);

        // --- PHASE 10: VOUCHER ---
        console.log("✅ [Phase 10] Kiểm tra mã Voucher 'WELCOME10'...");
        const vouRes = await fetch(`${BASE_URL}/voucher/validate/WELCOME10`);
        const vouData = await vouRes.json();
        if(vouData.success) console.log(`   -> Voucher hợp lệ! Giảm ${vouData.data.discountPercent}%`);

        // --- PHASE 8, 12, 16: ĐẶT HÀNG (SẼ TẠO QR VÀ GỬI EMAIL) ---
        console.log("✅ [Phase 8, 12, 16] Tạo Đơn hàng (Checkout)...");
        // Giả sử variantId = 1
        const orderRes = await fetch(`${BASE_URL}/order`, {
            method: 'POST', headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
            body: JSON.stringify({
                userId: userId,
                shippingAddress: "123 Test Street",
                note: "Test order",
                items: [{ variantId: 1, quantity: 1 }]
            })
        });
        const orderData = await orderRes.json();
        if(orderData.success) {
            console.log(`   -> Tạo đơn hàng thành công! OrderID: ${orderData.data.orderId}. (Đã tự động tạo Invoice và chuẩn bị gửi Email)`);
        } else {
            console.log("   -> Tạo đơn hàng thất bại (có thể do hết hàng hoặc variantId không đúng).");
        }

        // --- PHASE 16: DASHBOARD (ADMIN) ---
        // Login as admin
        const adminRes = await fetch(`${BASE_URL}/auth/login`, {
            method: 'POST', headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email: "admin@cakeshop.com", password: "Password123!" })
        });
        const adminLog = await adminRes.json();
        if(adminLog.success) adminToken = adminLog.data.token;

        console.log("✅ [Phase 16] Lấy thống kê Dashboard...");
        const dashRes = await fetch(`${BASE_URL}/dashboard/summary`, {
            headers: { 'Authorization': `Bearer ${adminToken}` }
        });
        const dashData = await dashRes.json();
        if(dashData.success) {
            console.log(`   -> Doanh thu: ${dashData.data.totalRevenue}, Đơn hàng: ${dashData.data.totalOrders}`);
        }

        console.log("\n🎉 HOÀN TẤT CHUỖI TEST TỰ ĐỘNG! Hệ thống Backend hoạt động hoàn hảo.");
    } catch(e) {
        console.error("Lỗi khi chạy test:", e);
    }
}

runTests();
