using SQLite;

namespace SleepHaven;

internal static class ProductSeedData
{
    public static IReadOnlyList<Product> All { get; } =
        new List<Product>
        {
                // Spring Products
                new Product
                {
                    Id = "p001",
                    Name = "\"Shidana\" Thick Thread 3D Jacquard Four-Piece Set",
                    Price = "$129.00",
                    Category = "BeddingSets - Spring",
                    Description = "Crafted from premium heavy-gauge 3D jacquard fabric, interlaced with imported silver threads. Exquisitely thick yet remarkably breathable, this set offers a soft, pressure-free embrace that feels like a \"second skin,\" gently cradling every moment of your fatigue.\r\n\r\n[ Fabric Features ]\r\nThe textured 3D jacquard perfectly embodies a premium quality that is both visible and tangible. The masterful combination of heavy-gauge weaving and silver threads awakens a sensory experience of full marks. High-count cotton yarns interwoven with imported silver radiance shimmer like fine sand under the sunlight, bringing the fabric’s \"Softness, Brilliance, and Smoothness\" to life.\r\n\r\nProduced on fully imported precision looms, our specialized manufacturing process elevates both the tactile feel and visual dimension—thick, fluffy, and delicate as clouds. The \"Heavenly Snow\" technique is applied to increase the yarn's loft while preserving its raw, silky texture, masterfully balancing the complexity of jacquard with the skin’s need for breathability. Enhanced with the latest ultra-soft, non-iron mercerizing process, it remains crisp and wrinkle-resistant.\r\n\r\n[ Color Palette ]\r\n\r\nSoft Champagne — Pale, mellow, and inherently gentle; a shade that inspires instant infatuation.\r\n\r\nDawn Cloud Grey — Light as a mist, blending cool composure with vintage elegance.\r\n\r\n[ Style & Craftsmanship ]\r\nPaneling & Mitred Corners + Embroidered Edging. Luxury is written in every thread. The substantial 8cm wide-border design, paired with structurally refined embroidery, creates a striking visual impact. It quietly narrates a story of craftsmanship and texture, outlining an elegant, multi-layered aesthetic.",
                    ThumbnailUrl = "shidana_folded.png",
                    LandscapeUrl = "shidana_bedroom.png",
                    IsFavorite = false
                },

                new Product
                {
                    Id = "p002",
                    Name = "Avaite Xinjiang Longyuan Cotton Bedding Set",
                    Price = "$299.00",
                    Category = "BeddingSets - Spring",
                    Description = "The long-staple cotton from Awaite in Xinjiang is exposed to 16 hours of ultra-long sunlight every day, which makes its cotton fibers longer and the spun fabric smoother, with a stronger luster, a more substantial and delicate feel, and a softer yet more resilient texture. The exquisite digital printing and dyeing process results in vivid and layered colors that remain unchanged even after repeated washing.",
                    ThumbnailUrl = "awati_long_staple_cotton_quilt_folded.png",
                    LandscapeUrl = "awati_long_staple_cotton_quilt_bedroom.png",
                    IsFavorite = false
                },

                // Summer products
                new Product
                {
                    Id = "p004",
                    Name = "Pure Silk Summer Quilt",
                    Price = "$189.00",
                    Category = "Quilts - Summer Sale",
                    Description = "60 pieces of Tencel fabric, including Lanxinye ultra-fine Tencel and long-staple cotton, with 7A antibacterial properties.\r\nClass A infant safety standard\r\nSOFT ultra-soft technology, smooth and soft to the touch\r\nHandmade embroidery, peony blossoms throughout the four seasons of spring, classic beauty\r\nThe silk contains 18 natural amino acids, which have anti-mite, antibacterial, anti-allergic, skin-friendly, breathable and fatigue-relieving effects.",
                    ThumbnailUrl = "silk_duvet_folded.png",  // Vertical screen/list-oriented folding diagram
                    LandscapeUrl = "silk_duvet_bedroom.png", // Portrait of a   bedroom in landscape mode
                    IsFavorite = false
                },

                new Product
                {
                    Id = "p003",
                    Name = "Tencel Cotton AB Dual-Purpose Bedding Set",
                    Price = "$220.00",
                    Category = "BeddingSets - Summer",
                    Description = "【Fabric Characteristics】Version A has a plain embroidered surface, while Version B features a Petchi pattern. Presented in modern line drawing style, with exquisite white starching edge work, it creates a three-dimensional jacquard texture. One side is gentle and elegant, while the other is elegant with patterns. Changing the side is like changing a bed. Both sides offer a pleasant view and can provide a good sleep experience.\r\nThe warp is 100% Lyocell, and the weft is 100% cotton. The warp and weft are interwoven, combining hardness and softness. Natural fibers give the fabric a soft and skin-friendly texture. It is soft and not stuffy, and is comfortable even when sleeping naked. The micro-level fine touch makes sleeping on it feel like being wrapped in clouds. The fabric has a pearl-like luster, and when illuminated, it has layers, like bringing afternoon sunlight into the bedroom.\r\n【Pattern Meaning】The Petchi pattern originated from the \"Tree of Life\" in ancient India, symbolizing endless life. The plain surface is dotted with the same color embroidery, symmetrical on both sides, echoing \"good things come in pairs\"; the printed surface has a recurring pattern, carrying the auspiciousness of \"completion\". A good product and good luck, Evilyn is an ideal choice for wedding and housewarming invitations.\r\n【Style and Craft】The double-sided multi-purpose design is a rational and exquisite style. The 8-centimeter wide border is pressed, and the hidden zipper at the end of the quilt. The double-sided aesthetics are timeless and colorful, remaining fresh as ever with the passage of time.\r\n【Design Inspiration】Derived from the combination of classic natural elements and modern minimalist home style, the design adopts a Petchi pattern with rhythmic beauty and romantic and classical charm, blending simplicity, softness, comfort, and healing together.",
                    ThumbnailUrl = "tencelcotton_abdual_use_folded.png",
                    LandscapeUrl = "tencelcotton_abdual_use_bedroom.png",
                    IsFavorite = false
                },


                // Autumn products
                new Product
                {
                    Id = "p005",
                    Name = "\"Dreams\" Printed Bedding Set",
                    Price = "$315.00",
                    Category = "Quilts - Autumn",
                    Description = "[Design Concept] Unbounded imagination, tracing the self. The artistic and aesthetically pleasing texture resembles a dreamlike world shrouded in mist. The soft beige tones blend with the lively and vibrant Tiffany blue, adding layers of beauty to the rose-gray powder, delicately interwoven in a way that makes one feel as if they are in an extraordinary dream, effortlessly conveying the ultimate sense of relaxation.\r\n[Fabric Characteristics] Version A uses high-count and high-density long-staple cotton printed jacquard, which has a flowing luster and skin-friendly properties, presenting a high-end and elegant feel while also magnifying the unique beauty of women to an unlimited extent, destined to be elegant. Interwoven with imported silver thread highlights, it shines brightly, as if it were a flowing visual feast, dazzling yet not ostentatious.\r\nVersion B uses Xinjiang Aweiti long-staple cotton, with the raw material grown in the 39° north latitude golden production area of Aweiti, with a fiber length of 38mm+. It has a soft light effect, and when turned over, it feels as if one is rolling among clouds.\r\n[Process Features] Edge-to-edge and corner-to-corner stitching + lace镶嵌. The super-wide edge design is elegant and composed. At the edge stitching， a romantic and glittering lace border is encountered， the edge is exquisite， like a French haute couture dress， lighting up the overall design.\r\n[Applicable Styles] Easily compatible with various home styles， with a \"quiet\" filter， it reduces anxiety， instantly elevating the bedroom style. Both minimalist and French-style bedrooms can be combined like model rooms.\r\nIn the busy life， there is always a need for a peaceful and relaxing space of one's own - when you can't roll it， just retreat into this gentle dreamland！",
                    ThumbnailUrl = "dream_folded.png",
                    LandscapeUrl = "dream_bedroom.png",
                    IsFavorite = false
                },
                // Winter products
                new Product
                {
                    Id = "p006",
                    Name = "Warm Flannel Duvet",
                    Price = "$320.00",
                    Category = "BeddingSets - Winter",
                    Description = "\"Fantasy Dream\" 120-thread thick woven three-dimensional jacquard four-piece set \r\n【Fabric Characteristics】The fabric is made of 120-count long-staple cotton from Xinjiang, with high-precision thick-weave jacquard weaving. It is produced by imported looms with precision. The fabric has a richer texture and a more exquisite pattern. The feel is as soft and smooth as clouds, thick and soft, with strong three-dimensionality. The exquisite thick-weave weaving process and the latest ultra-soft non-iron silk finishing process are added, making it resistant to wrinkles and well-shaped.\r\n【Process Features】Wide-edge perforation process + button design. The delicate perforation balances the gentle flower pattern with the neat lines, highlighting the exquisite sense of ceremony in the details.\r\nThe freehand peony flower design immediately caught my attention. The soft and gentle flower pattern in a warm golden tone, low-key yet elegant, perfectly matches my aesthetic preferences! The texture is even more outstanding when touched, and the durability is directly maximized. It can perfectly fit various modern, light luxury, or new Chinese styles. No additional soft furnishings are needed; just this set of bedding is enough to be stylish!\r\n【Design Creativity】Breaking the conventional regularity of three-dimensional color-weaving small compositions, with a three-dimensional relief dark pattern as the base, highlighting the dynamic and flexible nature of the golden jacquard.\r\nInspired by the peonies in spring, blooming on time, each flower is a beautiful note under the sunlight, feeling the beauty and freedom scattered in the air. Through exquisite jacquard techniques, the blooming peonies are brought into the bedroom, and each petal emits a brilliant and dazzling light. The peony is known as the \"king of flowers\", and since the Tang Dynasty, it has been highly praised. It is considered as the embodiment of wealth and prosperity, and is also called the \"wealthy flower\", symbolizing prosperous career, harmonious family, and continuous wealth. The exquisite detailing creates the peonies to be light and flexible, stunning at a glance.\r\n【Overall Colors】Low saturation, visually providing a very comfortable feeling. The two-color design, elegant French cream white, and the sophisticated and elegant retro pink, bright but not dazzling, gentle and elegant, giving people inner peace and spiritual cleansing. The embellishment of gold and silver threads in the fabric, like the unique light emitted by shells under the warm sun, makes the product present a unique elegant temperament.",
                    ThumbnailUrl = "fantasydream_folded.png",
                    LandscapeUrl = "fantasydream_bedroom.png",
                    IsFavorite = false
                },

                //Summer producets
                new Product
                {
                    Id = "p007",
                    Name = "Silk summer dress",
                    Price = "$299.00",
                    Category = "Quilts - Summer",
                    Description = "With silk as the medium, we weave the romance of summer into every inch of the fabric.\r\nThe three-dimensional embroidery flowers are delicately sewn onto the smooth Tencel bedding, and the glimmering colors showcase the elegance.\r\nThe inner layer is filled with natural mulberry silk, which is breathable, moisture-wicking, and lightweight without any pressure.\r\nEach time you turn over in the summer night, it brings the fragrance of flowers and the gentle touch of silk.\r\nSleep in a bed that breathes like a backyard garden.",
                    ThumbnailUrl = "mulberrysilk_summerquilt_folded.png",
                    LandscapeUrl = "mulberrysilk_summerquilt_bedroom.png",
                    IsFavorite = false
                },

                //Winter products
                new Product
                {
                    Id = "p008",
                    Name = "Selina enjoys the Hungarian goose down quilt",
                    Price = "$899.00",
                    Category = "Quilts - Winter",
                    Description = "It is crafted through a high-precision jacquard weaving technique. Each texture tells a story of ingenious design, being delicate and exquisite. It highlights an extraordinary style in the details. It is the same model as that in the Rocafu store, inheriting the high-end quality and adding a touch of luxurious atmosphere to your bedroom. \r\nThe filling is 95% white goose down from Hungary, with a loftiness of 800+.\r\nIt is light and fluffy, as soft as clouds.\r\nThe high loftiness provides excellent warmth retention, quickly trapping heat,\r\npreventing the cold night from invading while offering no heavy oppressive feeling.\r\nAt the same time, it has excellent breathability, quickly expelling moisture,\r\nensuring the bedding remains dry and comfortable at all times. \r\nA choice for the upper class, a sleep option for restful slumber. By choosing it, you are opting for a deep and luxurious sleep every night..",
                    ThumbnailUrl = "selina_folded.png",
                    LandscapeUrl = "selina_bedroom.png",
                    IsFavorite = false
                },

                //Winter product
                 new Product
                {
                    Id = "p009",
                    Name = "BellaLoft",
                    Price = "$39.00",
                    Category = "Pillows - Winter",
                    Description = "This product features a box-style partition design, filled with 1200g of Beibei soft and fluffy fiber. (Each fiber is lubricated with organic silicone oil, effectively alleviating the sensitive perception of microatoms when pressure and friction occur, which can effectively sense changes in pressure; the fibers are soft, fluffy, breathable, and moisture-absorbent. The fibers are even more soft and pliable under force.) This increases the fluffy space within the pillow core, making it durable and not prone to losing its cotton.\r\nThe pillow core is approximately 8cm in height, designed to meet the needs of those who prefer to sleep on a soft high pillow. It naturally fits the head, compensating for the gap in the neck, and meeting the needs of side sleeping, back sleeping, and stomach sleeping.\r\nOne pillow offers two sleeping sensations. One side is cool, and the other is warm. The cool side uses a skin-friendly, high-elastic yoga fabric, leaving no sleep marks upon waking, and is extremely smooth and comfortable. The warm side uses camellia flower fabric, which is beneficial for skin moisturization, with a faint fragrance of Chanel No. 5, helping to calm and sleep throughout the night.\r\nThe inner layer selects silk and fibers, and adopts a scientific layered filling design. You can enjoy the nourishment of silk protein, making your sleep more comfortable.",
                    ThumbnailUrl = "bella_loft_folded.png",
                    LandscapeUrl = "bella_loft_bedroom.png",
                    IsFavorite = false
                },

                 //Autumn product
                 new Product
                {
                    Id = "p010",
                    Name = "Oatmeal Pillow",
                    Price = "$49.00",
                    Category = "Pillows - Autumn",
                    Description = "What is best suited to be unfolded in the dead of night is the mind and the neck.\r\nThe Maya Cotton Sleep Eco Buckwheat Pillow is like bringing a small piece of the field into the bedroom. The rough cotton and linen touch the skin, giving a breathing texture; the buckwheat shells rustle softly beside the ears, like rain hitting leaves. It doesn't cater to you; it merely supports you - that steady, plant-like support with a fresh aroma, allowing the neck to find its most natural angle. \r\nA peaceful night's sleep left my shoulders and neck feeling relaxed upon waking up.\r\nThe dream might have also picked up a hint of the fragrance of plants.",
                    ThumbnailUrl = "oatmealpillow_folded.png",
                    LandscapeUrl = "oatmealpillow_bedroom.png",
                    IsFavorite = false
                },

                 //Spring prooduct
                 new Product
                {
                    Id = "p011",
                    Name = "Washed cotton soft cushion",
                    Price = "$59.00",
                    Category = "Mattresses - Spring",
                    Description = "100% cotton plain weave washed cotton, breathable and supportive",
                    ThumbnailUrl = "washed_cottonmat_folded.png",
                    LandscapeUrl = "washed_cottonmat_bedroom.png",
                    IsFavorite = false
                },

                 //Spring project
                 new Product
                {
                    Id = "p012",
                    Name = "Restful Recovery Memory Pillow",
                    Price = "$139.00",
                    Category = "Pillows - Spring",
                    Description = "High-performance racing seat-style memory pillow, revolutionizing the sleep experience of ordinary memory pillows Product selling points:\r\n1. The core is imported from Italy. The inner core is stamped with the Italian word \"ITALIA\" in steel. It is produced by the top Italian manufacturer of memory foam, \"TG\".\r\n2. Extremely soft and supportive, flexible support, head and neck relief, relaxation and deep sleep.\r\n3. Extremely breathable, with 1300 ventilation holes, full pillow ventilation, and efficient sweat removal.\r\n4. Super safe, in accordance with EU standards, odorless and hypoallergenic.\r\n(a. Certified by German TÜV Rheinland, the only threshold for products entering Germany!\r\n(b. Certified by the \"Environmental Oscar\" of the textile industry, Oeko-Tex Standard 100\r\n(c. CertiPUR, the \"green safety - gold standard certification\" for memory foam)\r\n5. Double-sided sleep sensation, one side is cool for sleep assistance, and the other side is warm and skin-friendly.",
                    ThumbnailUrl = "restful_recovery_memory_pillow_folded.png",
                    LandscapeUrl = "restful_recovery_memory_pillow_bedroom.png",
                    IsFavorite = false
                },

                 //Autumn project
                 new Product
                {
                    Id = "p013",
                    Name = "Zen Carbon Sleep Pad",
                    Price = "$199.00",
                    Category = "Mattresses - Autumn",
                    Description = "1. Adopting a brand-new seven-layer filling structure design (antibacterial and warm layer, spinal memory layer, bamboo charcoal particle layer, support layer, bamboo charcoal fiber layer, shock absorption layer, shaping layer), it not only provides comfort but also has a great appearance and functionality.\r\n2. On the basis of the original design, we have innovatively added five memory foam zones, which has brought a qualitative leap in comfort! It not only effectively supports the head, shoulders, waist, hips, and legs, but also provides a perfect sense of envelopment, allowing the human body to maintain the most comfortable and relaxed state.\r\n3. The bamboo charcoal particle layer and the patented carbon card filling have the dual functions of adsorbing odors and formaldehyde.\r\n4. The suede fabric is comfortable and anti-slip.\r\nNote: Three patent technologies\r\nPatent 1 - Five Zone Memory Foam\r\nPatent 2 - Carbon Card\r\nPatent 3 - Anti-Folding",
                    ThumbnailUrl = "bamboo_charcoal_mattress_folded.png",
                    LandscapeUrl = "bamboo_charcoal_mattress_bedroom.png",
                    IsFavorite = false
                },
        };
}
