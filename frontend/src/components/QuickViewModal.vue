<template>
  <div class="fixed inset-0 z-[100] flex items-center justify-center bg-black/50 backdrop-blur-sm p-4" @click.self="close">
    <div class="bg-surface w-full max-w-3xl rounded-2xl overflow-hidden shadow-2xl animate-fade-down flex flex-col md:flex-row relative max-h-[90vh]">
      
      <!-- Close Button -->
      <button @click="close" class="absolute top-4 right-4 z-10 bg-surface/80 text-on-surface-variant hover:text-error hover:bg-error/10 w-8 h-8 rounded-full flex items-center justify-center transition-all">
        <span class="material-symbols-outlined text-[20px]">close</span>
      </button>

      <!-- Image Section -->
      <div class="w-full md:w-1/2 bg-surface-container-low min-h-[250px] relative">
        <img v-if="product.imageUrl || product.variants?.[0]?.imageUrl" 
             :src="imgUrl(product.imageUrl || product.variants[0].imageUrl)" 
             :alt="product.name" 
             class="w-full h-full object-cover absolute inset-0" />
        <div v-else class="w-full h-full flex items-center justify-center text-on-surface-variant/30 absolute inset-0">
          <span class="material-symbols-outlined text-[80px]">bakery_dining</span>
        </div>
      </div>

      <!-- Content Section -->
      <div class="w-full md:w-1/2 p-6 md:p-8 flex flex-col overflow-y-auto">
        <h2 class="text-2xl font-bold text-on-surface mb-2">{{ product.name }}</h2>
        <p class="text-xl font-bold text-primary mb-6">{{ fmtMoney(selectedVariant ? selectedVariant.price : minPrice) }}</p>

        <!-- Variants Selection -->
        <div class="flex-1 flex flex-col gap-6">
          <div v-if="parsedTierVariations.length > 0" class="flex flex-col gap-4">
            <div v-for="(tier, tIdx) in parsedTierVariations" :key="tIdx" class="flex flex-col gap-2">
              <span class="font-bold text-on-surface text-sm">{{ tier.name }}:</span>
              <div class="flex flex-wrap gap-2">
                <button 
                  v-for="val in tier.values" 
                  :key="val" 
                  @click="selectTierValue(tIdx, val)"
                  :class="['px-4 py-2 rounded-lg font-medium border transition-colors text-sm', 
                    selectedTiers[tIdx] === val 
                      ? 'bg-primary text-white border-primary shadow-sm' 
                      : 'bg-surface text-on-surface-variant hover:border-primary hover:text-primary']"
                >
                  {{ val }}
                </button>
              </div>
            </div>
          </div>
          <div v-else-if="product.variants && product.variants.length > 1" class="flex flex-col gap-2">
            <span class="font-bold text-on-surface text-sm">Lựa chọn:</span>
            <div class="flex flex-wrap gap-2">
              <button 
                v-for="v in product.variants" 
                :key="v.variantId" 
                @click="selectedVariant = v"
                :class="['px-4 py-2 rounded-lg font-medium border transition-colors text-sm', 
                  selectedVariant?.variantId === v.variantId 
                    ? 'bg-primary text-white border-primary shadow-sm' 
                    : 'bg-surface text-on-surface-variant hover:border-primary hover:text-primary']"
              >
                {{ v.variantName }}
              </button>
            </div>
          </div>

          <!-- Description snippet -->
          <p class="text-sm text-on-surface-variant line-clamp-3 mt-2">
            {{ product.baseDescription || product.description }}
          </p>
        </div>

        <!-- Add to cart -->
        <div class="mt-6 pt-6 border-t border-outline-variant/30 flex flex-col gap-4">
          <div class="flex items-center gap-4">
            <div class="flex items-center border border-outline rounded-lg overflow-hidden bg-surface flex-shrink-0" :class="{'opacity-50 pointer-events-none': selectedVariant?.stockQty === 0}">
              <button @click="quantity > 1 && quantity--" class="w-10 h-10 flex items-center justify-center text-on-surface-variant hover:bg-surface-container transition-colors">
                <span class="material-symbols-outlined text-[18px]">remove</span>
              </button>
              <span class="w-10 text-center font-bold text-on-surface">{{ quantity }}</span>
              <button @click="quantity < (selectedVariant?.stockQty || 1) && quantity++" class="w-10 h-10 flex items-center justify-center text-on-surface-variant hover:bg-surface-container transition-colors">
                <span class="material-symbols-outlined text-[18px]">add</span>
              </button>
            </div>
            
            <button 
              @click="handleAddToCart" 
              :disabled="!selectedVariant || selectedVariant.stockQty === 0"
              :class="[
                'flex-1 py-3 px-4 rounded-lg font-bold transition-all flex items-center justify-center gap-2',
                (!selectedVariant || selectedVariant.stockQty === 0)
                  ? 'bg-surface-variant text-on-surface-variant cursor-not-allowed opacity-60' 
                  : 'bg-primary text-white hover:opacity-90 shadow-md shadow-primary/20'
              ]"
            >
              <span class="material-symbols-outlined" v-if="!selectedVariant || selectedVariant.stockQty === 0">remove_shopping_cart</span>
              <span class="material-symbols-outlined" v-else>add_shopping_cart</span>
              {{ (!selectedVariant || selectedVariant.stockQty === 0) ? 'Hết hàng' : 'Thêm vào giỏ' }}
            </button>
          </div>
          <router-link :to="`/product/${product.productId}`" class="text-center text-sm text-primary font-medium hover:underline">
            Xem chi tiết sản phẩm
          </router-link>
        </div>

      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useCartStore } from '../stores/cartStore';
import { toast } from 'vue3-toastify';

const props = defineProps({
  product: { type: Object, required: true }
});
const emit = defineEmits(['close']);

const cart = useCartStore();

const quantity = ref(1);
const selectedVariant = ref(null);
const parsedTierVariations = ref([]);
const selectedTiers = ref([]);

const imgUrl = url => url?.startsWith('http') ? url : `${import.meta.env.VITE_BASE_URL}${url}`;
const fmtMoney = v => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v);

const minPrice = computed(() => {
  const prices = props.product.variants?.map(v => v.price) || [];
  return prices.length ? Math.min(...prices) : 0;
});

const close = () => {
  emit('close');
};

const selectTierValue = (tIdx, val) => {
  selectedTiers.value[tIdx] = val;
  updateSelectedVariantFromTiers();
};

const updateSelectedVariantFromTiers = () => {
  if (!props.product || !props.product.variants) return;
  const comboStr = JSON.stringify(selectedTiers.value);
  selectedVariant.value = props.product.variants.find(v => {
     let vals = [];
     try { if (v.tierValuesJson) vals = JSON.parse(v.tierValuesJson); } catch(e){}
     return JSON.stringify(vals) === comboStr;
  });
};

const handleAddToCart = () => {
  const variant = selectedVariant.value || props.product.variants?.[0];
  if (!variant || variant.stockQty <= 0) {
    toast.error('Sản phẩm tạm hết hàng!');
    return;
  }
  for(let i=0; i<quantity.value; i++) {
    cart.addItem(props.product, variant);
  }
  toast.success(`Đã thêm ${quantity.value} "${props.product.name}" vào giỏ hàng!`);
  close();
};

onMounted(() => {
  if (props.product.tierVariationsJson) {
    try {
      const raw = JSON.parse(props.product.tierVariationsJson);
      parsedTierVariations.value = raw.map(t => ({
         name: t.name || t.Name || '',
         values: t.values || t.Values || []
      }));
      parsedTierVariations.value.forEach(t => selectedTiers.value.push(t.values[0] || ''));
      updateSelectedVariantFromTiers();
    } catch(e) {}
  }
  if (parsedTierVariations.value.length === 0 && props.product.variants?.length > 0) {
    selectedVariant.value = props.product.variants[0];
  }
});
</script>

<style scoped>
.animate-fade-down {
  animation: fade-down 0.3s ease-out;
}
@keyframes fade-down {
  from { opacity: 0; transform: translateY(-20px) scale(0.95); }
  to { opacity: 1; transform: translateY(0) scale(1); }
}
.line-clamp-3 {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;  
  overflow: hidden;
}
</style>
